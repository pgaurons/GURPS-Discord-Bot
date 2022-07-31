using System;

namespace Gao.Gurps.Model
{
    public interface IExpression<T>
    {

        T Evaluate();
        string PrintGraph();
        Guid UniqueIdentifier { get; }

        T Result { get; }

    }

    public class UnaryExpression<T> : IExpression<T>
    {

        public string PrettyPrint { get; set; }
        public IExpression<T> Operand;
        public Func<T, T> Function { get; set; }
        public Guid UniqueIdentifier { get; } = Guid.NewGuid();
        public T Evaluate()
        {
            return Result = Function(Operand.Evaluate());
        }

        public T Result
        {
            get;set;
        }

        public override string ToString()
        {
            return PrettyPrint ?? base.ToString();
        }



        public string PrintGraph()
        {
            return $"\"{UniqueIdentifier}\"[label = \"(Paranthesis)\"]" + Environment.NewLine + Operand.PrintGraph() + $"\"{UniqueIdentifier}\"->\"{Operand.UniqueIdentifier}\"" + Environment.NewLine;
        }
    }

    public class BinaryExpression<T> : IExpression<T>
    {
        public T Result
        {
            get; set;
        }
        public char PrettyPrintOperator { get; set; }
        public string PrettyPrint { get; set; }
        public IExpression<T> FirstOperand;
        public IExpression<T> SecondOperand;
        public Guid UniqueIdentifier { get; } = Guid.NewGuid();
        public Func<T, T, T> Function { get; set; }
        public T Evaluate()
        {
            return Result = Function(FirstOperand.Evaluate(), SecondOperand.Evaluate());
        }

        public override string ToString()
        {
            return PrettyPrint != null ? PrettyPrint : base.ToString();
        }

        public string PrintGraph()
        {
            return $"\"{UniqueIdentifier}\"[label = \"{PrettyPrintOperator}\"]" + Environment.NewLine + FirstOperand.PrintGraph() + SecondOperand.PrintGraph() + $"\"{UniqueIdentifier}\"->\"{FirstOperand.UniqueIdentifier}\"" + Environment.NewLine + $"\"{UniqueIdentifier}\"->\"{SecondOperand.UniqueIdentifier}\"" + Environment.NewLine;
        }
    }

    public class IdentityExpression<T> : IExpression<T>
    {
        public T Result
        {
            get; set;
        }
        public T Operand { get; set; }
        public Guid UniqueIdentifier { get; } = Guid.NewGuid();
        public T Evaluate()
        {
            return Result = Operand;
        }

        public override string ToString()
        {
            return Operand.ToString();
        }
        public string PrintGraph()
        {
            return $"\"{UniqueIdentifier}\"[label = \"{Operand}\"]" + Environment.NewLine;
        }
    }
}