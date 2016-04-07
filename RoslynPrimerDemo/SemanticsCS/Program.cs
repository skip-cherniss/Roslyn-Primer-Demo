using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// This example is from the Roslyn Wiki
/// https://github.com/dotnet/roslyn/wiki/Getting-Started-C%23-Semantic-Analysis
/// 
/// Example - Creating a compilation
/// This example shows how to create a Compilation by adding assembly references 
/// and source files.Like the syntax trees, everything in the Symbols API 
/// and the Binding API is immutable.
/// 
/// Example - Binding a name
/// This example shows how to obtain a SemanticModel object for our HelloWorld 
/// SyntaxTree.Once the model is obtained, the name in the first using directive 
/// is bound to retrieve a Symbol for the System namespace.
/// 
/// I have blended the notes in with the code sample
/// There are data tips pinned to the page exposing the properties to examine at run time
/// If they do not display, import them from the Debug Menu - File "SemanticCS Data Tips.xml"
/// 
/// </summary>
namespace SemanticsCS
{
    class Program
    {
        static void Main(string[] args)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(
@" using System;
using System.Collections.Generic;
using System.Text;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(""I have a particular set of skills!"");
        }
    }
}");
            // Example - Creating a compilation
            var root = (CompilationUnitSyntax)tree.GetRoot();

            var compilation = 
                CSharpCompilation
                    .Create("Taken")
                    .AddReferences(
                        // Original Reference Code
                        // This method was depreciated
                        //MetadataReference.CreateFromAssembly(typeof(object).Assembly.Location))
                        MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                    .AddSyntaxTrees(tree);

            // Once you have a Compilation you can ask it for a 
            // SemanticModel for any SyntaxTree contained in that 
            // Compilation. SemanticModels can be queried to answer 
            // questions like "What names are in scope at this location?" 
            // "What members are accessible from this method?" 
            // "What variables are used in this block of text?" 
            // "What does this name/expression refer to?"
            var model = compilation.GetSemanticModel(tree);

            // Note the Symbol property. 
            // This method uses returns the Symbol referenced by the expression "System". 
            // 
            // For expressions which don't refer to anything (such as numeric literals) 
            // this property will be null.
            // Note that the Symbol.Kind property returns the value SymbolKind.Namespace.



            var nameInfo = model.GetSymbolInfo(root.Usings[0].Name);
            



            var systemSymbol = (INamespaceSymbol)nameInfo.Symbol;



            // Example - Binding a name

            // Even though this symbol is declared in outside code
            // The dll exposes the namespace members and roslyn can
            // reference them
            foreach (var ns in systemSymbol.GetNamespaceMembers())
            {
                Console.WriteLine(ns.Name);
            }

            // Retrieve are string literal
            var skillsString = root.DescendantNodes()
                                       .OfType<LiteralExpressionSyntax>()
                                       .First();




            
            // Retrieve the Type Info
            var literalInfo = model.GetTypeInfo(skillsString);

            
            var stringTypeSymbol = (INamedTypeSymbol)literalInfo.Type;

            // Retrieve a reference to the System Identifier
            var systemIdentifierSearch =
                root.Usings
                    .Select(uds =>
                        uds.Name.DescendantNodes().OfType<IdentifierNameSyntax>())
                    .SelectMany(x => x)
                    .Where(ins => ins.Identifier.ValueText == "System")
                    //.Select(ins => ins.Identifier) // Returns Token
                    .First();

            // Does Not use Token
            // Use the System Identifier Name Syntax SyntaxNode to get the Symbol Info
            var systemTypeSymbolInfo = 
                model.GetSymbolInfo(systemIdentifierSearch);
            
            // Use the Symbol Info to get the Symbol
            var systemIdentifierSearchSymbol = (INamespaceSymbol)systemTypeSymbolInfo.Symbol;

            // This is an example of how you can filter types
            if (literalInfo.Type.Name == typeof(string).Name && 
                literalInfo.Type.ContainingNamespace.Name == typeof(string).Namespace &&
                //root.Usings.Select(uds => uds.Name.DescendantNodes().OfType<IdentifierNameSyntax>()).SelectMany(x => x).Where(ins => ins.Identifier.ValueText == "System").Any()
                stringTypeSymbol.ContainingSymbol == systemIdentifierSearchSymbol
                )
            {
                System.Console.WriteLine("I will find you!!!");
            }

            Console.Clear();
            foreach (var name in (from method in stringTypeSymbol.GetMembers()
                                                              .OfType<IMethodSymbol>()
                                  where method.ReturnType.Equals(stringTypeSymbol) &&
                                        method.DeclaredAccessibility ==
                                                   Accessibility.Public
                                  select method.Name).Distinct())
            {
                Console.WriteLine(name);
            }
        }
    }
}