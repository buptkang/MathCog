fslex.exe "$(ProjectDir)TextLexer.fsl" --unicode
fsyacc.exe "$(ProjectDir)TextParser.fsy" --module starPadSDK.MathExpr.TextInternals.Parser --open starPadSDK.MathExpr
