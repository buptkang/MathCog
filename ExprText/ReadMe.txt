fsyacc --module starPadSDK.MathExpr.TextInternals.Parser --open starPadSDK.MathExpr -v "$(ProjectDir)TextParser.fsy"

fslex --unicode TextLexer.fsl