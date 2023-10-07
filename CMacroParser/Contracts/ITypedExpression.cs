using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMacroParser.Contracts
{
    internal interface ITypedExpression : IExpression
    {
        string GetExpressionType();


        //#define INT_MAX 10        -> 1. Name: INT_MAX, 2. ConstantExpression: 10
        //#define INT_MIN -INT_MAX  -> 1. Name: INT_MIN, 2. UnaryOperatorExpression->VariableExpression INT_MAX
        //#define MIN(NUM) NUM      -> 1. Name: 


    }
}
