http://www.wired.com/2015/05/artificial-intelligence-pioneer-concerns/

https://github.com/aima-java/aima-java

# Read Me #
"Rome wasn't built in a day"

Logic Programming

Relation Programming

Pattern Matching

Parallel Knowledge Representation

Constraint Solving

Interactive Reasoning

AG.Reasoner is the logic inference engine with the long term memory.

Supported Trace Type:
Arithmetic trace step
Algebraic trace step 
Substitution trace step
Geometry trace step
Connection trace step between Geometry and Algebra



Getting Started

Propositional calculus
https://en.wikipedia.org/wiki/Propositional_calculus

Propositional logic (also called sentential logic) is the logic the includes sentence letters (A,B,C) and logical connectives, but not quantifiers. The semantics of propositional logic uses truth assignments to the letters to determine whether a compound propositional sentence is true.

Predicate logic is usually used as a synonym for first-order logic, but sometimes it is used to refers to other logics that have similar syntax. Syntactically, first-order logic has the same connectives as propositional logic, but it also has variables for individual objects, quantifiers, symbols for functions, and symbols for relations. The semantics include a domain of discourse for the variables and quantifiers to range over, along with interpretations of the relation and function symbols.

First order logic

This library is mainly focus on inference based on the relation graph of 
geometry entity, relation and properties.

Graph Unification Procedure

Constraint-based Programming

Input Uncertainty
Fuzzy Logic Versus Bayesian Inference

Information Extraction base on rule-base information system:
Fuzzy Logic
Bayesian Logic

https://dslpitt.org/genie/index.php/about

Bayesian inference to filter

http://www.codeproject.com/Articles/3320/Inference-in-Belief-Networks

https://github.com/ArdaXi/Bayes.NET

https://github.com/danielkorzekwa/bayes-scala

Constraint solving in Logical relation programming:

Constraints interpreted as goals

How to interoperate with a constraint solver here, such as a SAT solver?
https://github.com/lifebeyondfife/Decider

http://labix.org/doc/constraint/

http://labix.org/python-constraint

https://pypi.python.org/pypi/python-constraint

https://github.com/google/or-tools

ShapeNode four states 
=====================================================
|	ShapeNode		|  Concrete    | Non-Concrete   | 
=====================================================
|Relation based     |     C        |       D        |
-----------------------------------------------------
|Non-Relation based |     A        |       B        |
=====================================================

Concrete means the object does not contain any variable, 
non-concrete vice versa.

The transition between concrete and non-concrete means:
entity internal unify and reify process.

The transition between Relation based and Non-Relation based means:
entity external unify and reify process.

Graph Search Algorithm (Heuristic Search on directed acyclic graph):

//Symbolic Reification
void Reification(Goal goal)
{
    foreach(GraphNode node in Nodes) //bfs
	{
		//check node is ShapeNode
		if(reify(node, goal)) // B -> A
		{
			ReifyShapeNodeByRelation(node); //dfs, recursive D -> C
		}
	}
}

//Symbolic Unification
void Unification(object obj)
{
	Goal goal = obj as Goal;
	if(goal != null)
	{
		Reification(goal);
		UpdateRelation(goal); //build relation with relation-based obj
		return;
	}
	
	Shape shape = obj as Shape;
	if(shape is non-relation based obj)
	{
		Reify(shape)// reify itself, search other goals
		UpdateRelation(shape); //build relation with relation-based obj
	}
	else // shape is relation based obj
	{
		UnifyRelation(shape);//build relation with non-relation based obj
							 //TODO: build relation with relation based obj
	}
}