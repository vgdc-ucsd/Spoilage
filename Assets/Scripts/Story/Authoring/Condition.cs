using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player-state quantity that a <see cref="ConditionLeaf"/> reads against a
/// constant value.
/// </summary>
public enum ConditionField
{
    Day,
    ResistanceScore,
    SemiKeyRefusedToday,
    SemiKeyRefusedLifetime,
    SemiImportantDishesFailedToday,
    SemiImportantDishesFailedLifetime,
}

/// <summary>Comparison applied between a <see cref="ConditionField"/> reading and a constant value.</summary>
public enum ComparisonOp
{
    Equals,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
}

/// <summary>
/// One node of a boolean expression tree evaluated against
/// <see cref="PlayerData"/>. Stored on authoring ScriptableObjects via
/// <c>[SerializeReference]</c>, which is what lets the Unity Inspector pick a
/// concrete subclass per node.
/// </summary>
/// <remarks>
/// A null root counts as <c>true</c> (the identity for AND), so a
/// <see cref="RadioEntry"/> or <see cref="ReactionaryRule"/> with no
/// condition is always eligible.
/// </remarks>
[Serializable]
public abstract class ConditionNode
{
    public abstract bool Evaluate(PlayerData state);

    /// <summary>Null-safe entry point. A null tree is treated as <c>true</c>.</summary>
    public static bool EvaluateOrTrue(ConditionNode root, PlayerData state)
    {
        return root == null || root.Evaluate(state);
    }
}

/// <summary>Atomic comparison <c>field op value</c>.</summary>
[Serializable]
public class ConditionLeaf : ConditionNode
{
    public ConditionField field;
    public ComparisonOp op;
    public float value;

    public override bool Evaluate(PlayerData state)
    {
        float read = ReadField(state, field);
        return op switch
        {
            ComparisonOp.Equals => read == value,
            ComparisonOp.GreaterThan => read > value,
            ComparisonOp.GreaterThanOrEqual => read >= value,
            ComparisonOp.LessThan => read < value,
            ComparisonOp.LessThanOrEqual => read <= value,
            _ => false,
        };
    }

    private static float ReadField(PlayerData state, ConditionField field) => field switch
    {
        ConditionField.Day => state.Day,
        ConditionField.ResistanceScore => state.resistanceScore,
        ConditionField.SemiKeyRefusedToday => state.semiKeyRefusedToday,
        ConditionField.SemiKeyRefusedLifetime => state.semiKeyRefusedLifetime,
        ConditionField.SemiImportantDishesFailedToday => state.semiImportantDishesFailedToday,
        ConditionField.SemiImportantDishesFailedLifetime => state.semiImportantDishesFailedLifetime,
        _ => 0f,
    };
}

/// <summary>Holds when the player has given <see cref="itemId"/> to <see cref="recipientId"/>.</summary>
[Serializable]
public class ConditionItemExchange : ConditionNode
{
    public string itemId;
    public string recipientId;

    public override bool Evaluate(PlayerData state)
    {
        if (state == null || state.itemExchanges == null) return false;
        for (int i = 0; i < state.itemExchanges.Count; i++)
        {
            ItemExchange e = state.itemExchanges[i];
            if (e != null && e.itemId == itemId && e.recipientId == recipientId) return true;
        }
        return false;
    }
}

/// <summary>Holds when every child holds. An empty list holds (the AND identity).</summary>
[Serializable]
public class ConditionAnd : ConditionNode
{
    [SerializeReference] public List<ConditionNode> children = new List<ConditionNode>();

    public override bool Evaluate(PlayerData state)
    {
        for (int i = 0; i < children.Count; i++)
        {
            if (!EvaluateOrTrue(children[i], state)) return false;
        }
        return true;
    }
}

/// <summary>Holds when at least one child holds. An empty list does not hold.</summary>
[Serializable]
public class ConditionOr : ConditionNode
{
    [SerializeReference] public List<ConditionNode> children = new List<ConditionNode>();

    public override bool Evaluate(PlayerData state)
    {
        for (int i = 0; i < children.Count; i++)
        {
            if (EvaluateOrTrue(children[i], state)) return true;
        }
        return false;
    }
}

/// <summary>Negates a single child.</summary>
[Serializable]
public class ConditionNot : ConditionNode
{
    [SerializeReference] public ConditionNode child;

    public override bool Evaluate(PlayerData state)
    {
        return !EvaluateOrTrue(child, state);
    }
}

/// <summary>Holds when exactly one child holds.</summary>
[Serializable]
public class ConditionXor : ConditionNode
{
    [SerializeReference] public List<ConditionNode> children = new List<ConditionNode>();

    public override bool Evaluate(PlayerData state)
    {
        int trueCount = 0;
        for (int i = 0; i < children.Count; i++)
        {
            if (EvaluateOrTrue(children[i], state))
            {
                trueCount++;
                if (trueCount > 1) return false;
            }
        }
        return trueCount == 1;
    }
}
