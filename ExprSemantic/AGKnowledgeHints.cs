using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGSemantic.KnowledgeBase;

namespace ExprSemantic
{
    public class AGKnowledgeHints
    {
        #region Knowledge and Knowledge Property Hint

        public const string LineGeneralFormHint = "The general form of a line is Ax + By + C = 0.";
        public const string LineCoefficientHint = "As the general form of a line is Ax + By + C = 0, you can get three coefficients respectively.";
        public const string LineSlopeHint = "For a general form line Ax + By + c = 0, its slope is m = -A/B";

        public const string FindlineYInterceptPoint = "Find the Y-Intercept of the line.";

        public const string FitYInterceptIntoPointSlopeForm =
            "Substitute the value of Y-Intercept Point into point-slope form.";

        public const string LineYInterceptHints = "For a general form line Ax + By + c = 0, its Y intercept is I = -C/B";

        public const string CircleStandardFormHint = "The standard form of a circle is (x-a)^2 + (y-b)^2 = c^2.";
        public const string CircleRadiusHint = "For a standard form circle (x-a)^2 + (y-b)^2 = c^2, its radius is c.";
        public const string CircleCentralPtHint = "For a standard form circle (x-a)^2 + (y-b)^2 = c^2, its center is CP(a,b).";

        public const string EllipseStandarFormHint = "The standard form of a ellipse is (x-x0)/a^2 + (y-y0)/b^2 = 1";
        public const string EllipseCenterHint = "For a standard form ellipse  (x-x0)/a^2 + (y-y0)/b^2 = 1, its center is CP(a,b).";
        public const string EllipseRadiusHint = "For a standard form ellipse  (x-x0)/a^2 + (y-y0)/b^2 = 1, its semimajor is a, semiminor is b";
        public const string EllipseFociHint = "For a standard form ellipse (x-x0)/a^2 + (y-y0)/b^2 = 1, its foci is c^2 = a^2 - b^2";
        public const string EllipseFociPoint = "For a standard form ellipse with foci c, FP1 is (-c+a,b), FP2 is (c+a,b)";

        public const string ImplicitLineHint = "Implicit Line Form: aX + bY + c = 0";
        public const string ExplicitLineHint = "Explicit Line Form: Y = aX + b";
        public const string ParametricLineHint = "Parametric Line Form: X = x0 + a * T, Y = y0 + b * T";

        public const string ImplicitCircleHint = "Implicit Circle Form: (X-a)^2 + (Y-b)^2 = r^2";
        public const string ParametricCircleHint = "Parametric Circle Form: X = r * cos(t), Y = r * sin(t)";

        public const string ImplicitEllipseHint = "Implicit Ellipse Form: (X-h)^2/a^2 + (Y-k)^2/b^2 = 1";
        public const string ParametricEllipseHint = "Parametric Ellipse Form: X = a * cos(t) + h, Y = b * sin(t) + k";

        #endregion

        #region Algebra Manipulation Hint

        public const string FindIntercepts = "Find X-Intercept and Y-Intercept from Line Standard form.";

        public const string MoveTermsFromRightToLeft = "Move Terms from the Right Of Equatin to the Left.";
        public const string CommutativeLaw = "Find and Group Terms with the different Coefficients and same variable.";
        public const string MergeLaw = "Merge Terms with different Coefficients and same variable.";

        public const string FactorCircle0 = "Normalize Circle's Coefficients.";
        public const string FactorizeCircle1 = "Prepare to add the needed value to create the perfect square trinomial.";
        public const string FactorizeCircle2 = "Merge Constant Terms.";
        public const string FactorizeCircle3 = "Factor the perfect square trinomial.";
        public const string FactorizeCircle4 = "Move Constant Coefficient to the right of equation.";
        public const string FactorizeCircle5 = "Take the square root of each side and solve.";

        public const string FactorizeEllipse1 = "Normalize the Ellipse Constant Coefficient.";
        public const string FactorizeEllipse2 = "Calculate the Ellipse Constant Coefficient.";
        
        #endregion
    }

    public class AGStrategyHint
    {
        //problem 1, question 1
        public const string QueryLineStandardFormStrategyFromGeometry = "The general of line form is ax+by+c=0.";
        //problem 1, question 2
        public const string LineSlopeStrategy = "The slope of line Ax + By + C = 0 is – A/B";


        public const string DistanceBetweenTwoPoints = "The distance bwtween two points (x0,y0) and (x1,y1) is " +
                                                       "\n d^2 = (x0-x1)^2 + (y0-y1)^2";
                                            


        //Problem 4
        public const string DistanceBetweenPointAndLine =
            "The distance between a point (X0,Y0) and a line ax+by+c = 0 is" +
            "\n (|aX0 + bY0 + c| divide by Math.Sqrt(a^2 + b^2) )";



        public const string QueryLinePointSlopeFormStrategy = "A line with the point-slope form is like Y-Y0 = m(X-X0), " +
                                                              "\nwhere m is the slope, and (X0,Y0) is a point on the line.";
        public const string AlgebraicTransformation = "Transform this non-general form equation to a general form equation";

    }

    public class AGAppliedRule
    {
        public const string ConceptUnderstanding = "Extract Properties From Math Concepts.";
        public const string LineSlopeFromABC = "Get Line Slope from A, B, C of general line form.";
        public const string CalculateYIntercept = "Calculate the Y-Intercept of a line.";
        public const string FitYInterceptIntoPointSlopeForm = "Substitute Y-Intercept value into the Point-Slope form.";
        public const string AlgebraSubsitituion = "Algebra Substitution";
        public const string AlgebraMovingTerms = "Move Algebraic Terms";
        public const string AlgebraCommutativeLaw = "Apply Commutative Law";
        public const string AlgebraMergeLikeTerms = "Combine Like Terms";
        public const string AlgebraElimination = "Algebra Elimination";
        public const string FitABCToGeneralForm = "Substitute A,B,C into the line general form.";

        public const string FindIntercepts = "Let y=0 and x=0 respectively.";

    }

    public class AGWarning
    {
        public const string NoContentOnWhat = "You need to draw a question mark upon or near the expression to ask what it is.";
        public const string NoContentOnWhy  = "You need to select a problem or a question to ask why.";
        public const string NoContentOnHow  = "You need to select a problem or a question to ask how to do it.";
        public const string NoContentOnHowNext = "You need to draw the down arrow gesture under one expression.";

        public const string DownArrowUnderStrategy = "AnalyticalInk gives you the strategy to solve this problem. " +
                                                     "\nIf you know how to solve problem, further write next step below the strategy" +
                                                     "\nIf not, write a down-arrow below the strategy, AnalyticalInk will help you.";

        //public const string SelectQuestion = "What is the question? Input the question.";
        public const string SelectProblem = "What is the problem? Input the problem.";
        public const string NoQueryOnTheKnowledge = "Your query question does not fit any problem.";

        public const string InvalidAskContent = "Your asking is neither the problem nor the question.";
        
        public const string NoHint = "No Further Hint.";
        public const string FurtherDownArrowHint = "You can further draw a down arrow to let the system continue to help you.";
        public const string DoHowBeforeDownArrorwHint = "Circle-question this problem or question before letting system do derivation.";

        public const string NoProblems = "Few problem is detected. Input the problem first.";

    }


}
