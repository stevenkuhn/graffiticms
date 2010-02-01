namespace Graffiti.Core
{
    /// <summary>
    /// Baseline user values.
    /// </summary>
    public interface IUser
    {
        string Name { get; }

        string ProperName { get; set;}

        string WebSite { get; set; }

    }
}