using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.Interpreter
{
    public enum QueryMode
    {
        What, How, Why, Strategy,
        WhatOnKnowledge, HowOnKnowledge, WhyOnKnowledge, StrategyOnKnowledge,
        WhatOnProperty, HowOnProperty, WhyOnProperty, StrategyOnProperty,
        None
    }
}
