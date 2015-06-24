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

------------------------------------

Examples:
------------------------------------
A: 
1. a point "A(2,3)"
2. a line  "2x-4y+1=0"
-------------------------------------
B:
1. a point "A(x,3)"
2. a line  "ax-4y+1=0"
-------------------------------------
Relation means the connection(edge) between graph node on the graph.
Relation depends on the symbolic characters of entity.


Assumption: Relation-based object can even exist without the relation.
Reason: eg. If a symbol "AB" specifically means a line, then the individual 
character can be updated later to represent its sub-element.

Eg input sequence:
1. A line relation-based object "AB" => no relation
2. A point A(2,3) => create relation between "A" and "AB"
3. A point B(3,4) => create relation between "B" and "AB"
Satisfy Update (A,B) 
=> Unify relation-based object to Non-Relation based object.

--------------------------------------
C:
true positive Assumption: two points: A(2,3) and B(3,4)
1. a line "AB" 

false negative assumption: two popints: C(2,3) and B(3,4)
1. a line "AB"

---------------------------------------
D:
true positive Assumption: two points: A(2,x) and B(3,4)
1. a line "AB" 


===============================================================

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

===============================================================

Input Example:

Pattern Match Scenario 1: (deterministic)

Input sequence:
1: A(2,3) [Point] => Point
2: B(3,4) [Point] => Point
3: Hat(AB)[Line]  => Line

Input sequence:
1: Hat(AB) [Line]  => Line
2: A(2,3)          => Point
3: B(3,4)          => Point