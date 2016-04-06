using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UsingCollectorCS
{
    // The class inherits from CSharpSyntaxWalker
    // Use the Object Browser F12 to examine the CSharpSyntaxWalker
    // Notice it inherits from CSharpSyntaxVisitor
    // There are 187 Visitor Methods
    class UsingCollector : CSharpSyntaxWalker
    {
        public readonly List<UsingDirectiveSyntax> Usings = new List<UsingDirectiveSyntax>();

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            if (node.Name.ToString() != "System" &&
                !node.Name.ToString().StartsWith("System."))
            {
                this.Usings.Add(node);
            }
        }

        // Could Perform an Example where you record all variables and list their assignments
        // VisitAssignmentExpression
        // VisitVariableDeclarator
    }
}