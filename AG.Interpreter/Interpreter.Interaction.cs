namespace AG.Interpreter
{
    public partial class Interpreter : IInterpreter
    {
        #region Delegate for the interaction purpose

        public delegate void UpdateQueryHandler(object sender, object args);

        public event UpdateQueryHandler QueryUpdated;

        #endregion
    }
}
