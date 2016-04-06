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
/// https://github.com/dotnet/roslyn/wiki/Getting-Started-C%23-Syntax-Analysis
/// Example - Manually traversing the tree
/// I have blended the notes in with the code sample
/// There are data tips pinned to the page exposing the properties to examine at run time
/// If they do not display, import them from the Debug Menu - File "GettingStartedCS Data Tips.xml"
/// </summary>
namespace GettingStartedCS
{
    class Program
    {
        static void Main(string[] args)
        {
            
            SyntaxTree tree = CSharpSyntaxTree.ParseText(
@"using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(""Hello, World!"");
        }
    }
}");

            // Set Breakpoint Here
            // Advance to next line

            // Inspect the root variable in the debugger by hovering over it and expanding the datatip.
            //Note that its Usings property is a collection with four elements; one for each using 
            //directive in the parsed text.
            //Note that the KindText of the root node is CompilationUnit.
            //Note that the Members collection of the CompilationUnitSyntax node has one element.

            var root = (CompilationUnitSyntax)tree.GetRoot();

            // Hover over the firstMember variable and expand the datatips to inspect it.
            //Note that its KindText is NamespaceDeclaration.
            //Note that its run - time type is NamespaceDeclarationSyntax.

            var firstMember = root.Members[0];

            // Note that like the CompilationUnitSyntax, NamespaceDeclarationSyntax 
            // also has a Members collection.
            // Examine the members collection
            
            var helloWorldDeclaration = (NamespaceDeclarationSyntax)firstMember;
            
            //Note that it contains a single member.Examine it.
            //Note that its KindText is ClassDeclaration.
            //Note that its run - time type is ClassDeclarationSyntax.
            var programDeclaration = (ClassDeclarationSyntax)helloWorldDeclaration.Members[0];

            // Locate the Main declaration in the programDeclaration.Members collection 
            // and store it in a new variable:
            //Note the ReturnType, and Identifier properties.
            //Note the Body property.
            //Note the ParameterList property; examine it.
            //Note that it contains both the open and close parentheses of the parameter list 
            // in addition to the list of parameters themselves.
            
            var mainDeclaration = (MethodDeclarationSyntax)programDeclaration.Members[0];


            //Note the type of the argsParameter is ArrayType string[]
            //Note that the parameters are stored as a SeparatedSyntaxList<ParameterSyntax>.
            // mainDeclaration.ParameterList.Parameters.GetType().Name
            //Examine the Identifier property; note that it is of the structure type SyntaxToken.
            var argsParameter = mainDeclaration.ParameterList.Parameters[0];
            
            //Examine the properties of the Identifier SyntaxToken; note that the text of the identifier 
            // can be found in the ValueText property.
            var argIdentifierValueText = argsParameter.Identifier.ValueText;


            var argIdentifierTypeName = argsParameter.Identifier.GetType().Name;

            // Query Methods
            // In addition to traversing trees using the properties of the SyntaxNode 
            // derived classes you can also explore the syntax tree using the query methods 
            // defined on SyntaxNode. These methods should be immediately familiar to anyone 
            // familiar with XPath.You can use these methods with LINQ to quickly find 
            // things in a tree.

            //  Using IntelliSense, examine the members of the SyntaxNode class through the root variable.
            // Note query methods such as DescendantNodes, AncestorsAndSelf, and ChildNodes.

            // root.DescendantNodes()
            // root.AncestorsAndSelf()
            // root.ChildNodes()

            var firstParameters = 
                from methodDeclaration in root.DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                where methodDeclaration.Identifier.ValueText == "Main"
                select methodDeclaration.ParameterList.Parameters.First();

            var argsParameter2 = firstParameters.Single();

            // Using the Immediate window, type the expression 
            // argsParameter == argsParameter2 
            // and press enter to evaluate it.

            // Note that the LINQ expression found the same parameter as manually navigating the tree.
        }
    }
}