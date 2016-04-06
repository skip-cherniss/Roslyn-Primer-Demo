using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// This Example is from the Roslyn Wiki
/// https://github.com/dotnet/roslyn/wiki/Getting-Started-C%23-Syntax-Analysis
/// Often you'll want to find all nodes of a specific type in a syntax tree, 
/// for example, every property declaration in a file. By extending the 
/// CSharpSyntaxWalker class and overriding the VisitPropertyDeclaration method 
/// you can process every property declaration in a syntax tree without knowing 
/// its structure beforehand. CSharpSyntaxWalker is a specific kind of SyntaxVisitor 
/// which recursively visits a node and each of its children.
///
///Example - Implementing a SyntaxWalker
///
///This example shows how to implement a CSharpSyntaxWalker which examines an 
///entire syntax tree and collects any using directives it finds which aren't 
///importing a System namespace.
///
/// Execute the program and observe that the walker has located all 
/// non-System namespace using directives in all four places.
/// </summary>

namespace UsingCollectorCS
{
    class Program
    {
        static void Main(string[] args)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace TopLevel
{
    using Microsoft;
    using System.ComponentModel;

    namespace Child1
    {
        using Microsoft.Win32;
        using System.Runtime.InteropServices;

        class Foo { }
    }

    namespace Child2
    {
        using System.CodeDom;
        using Microsoft.CSharp;

        class Bar { }
    }
}");

            var root = (CompilationUnitSyntax)tree.GetRoot();

            var collector = new UsingCollector();
            collector.Visit(root);

            foreach (var directive in collector.Usings)
            {
                Console.WriteLine(directive.Name);
            }
        }
    }
}