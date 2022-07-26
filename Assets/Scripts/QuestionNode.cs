using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionNode : INode
{
    public delegate bool myDelegate();
    myDelegate question;
    INode trueNode;
    INode falseNode;
    public QuestionNode(myDelegate Question, INode TrueNode, INode FalseNode) 
    {
        question = Question;
        trueNode = TrueNode;
        falseNode = FalseNode;
    }
    public void Execute()
    {
        if (question())
        {
            trueNode.Execute();
        }
        else 
        {
            falseNode.Execute();
        }
    }
}
