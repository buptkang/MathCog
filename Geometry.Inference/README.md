This library is mainly focus on inference based on the relation graph of 
geometry entity, relation and properties.

Graph Unification Procedure

Constraint-based Programming

Input Uncertainty
Fuzzy Logic Versus Bayesian Inference

Information Extraction base on rule-base information system:
Fuzzy Logic
Bayesian Logic
==========================================================
Pattern Match Scenario 1:

Input sequence:
1: A(x,2) [Point]     => Point
2: x=2 [Goal, Line]   => Goal

Input sequence:
1: x=2 [Goal, Line]   => Line

Input sequence:
1: x=2 [Goal, Line]      => Line
2: A(x,2) [Point]        => Point 
Update: x=2 [Goal, Line] => Goal 

===========================================================

Pattern Match Scenario 2:

Input sequence:
1: A(2,3) [Point] => Point
2: B(3,4) [Point] => Point
3: AB [Label]     => [Line, LineSegment]
4: User Input to solve uncertainty

Input sequence:
1: AB [Label]      => [Label]
2: A(2,3)          => Point
3: B(3,4)          => Point
Update: AB [Label] => [Line, LineSegment]
4: User Input to solve uncertainty

4: AB[Label]      => LineSegment
5: d[Label] => distance of LineSegment
===========================================================

Pattern Match Scenario 3:

Input sequence:
1: A(2,3) [Point] => Point
2: B(3,4) [Point] => Point
3: AB[Label]      => [Line, LineSegment]
4: User Input to solve uncertainty:
   AB[Label]      => Line
5: d[Label]       => Label

Inference
1. Shape Entity:   e.g general form of line
2. Shape Property: the slope of line
3. Shape Relation: e.g given slope and intercept, construct the line.

Paramter Goal Constraints:
1.Parameter: Goal or Label, (optional ShapeType)
2.Parameter: Goal or ,      (optional ShapeType)
3.Parameter: (optional Goal or Label), ShapeType