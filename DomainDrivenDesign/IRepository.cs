namespace DDD
{
    /// <summary>
    /// For each type of object that needs global access, create an object 
    /// that can provide the illusion of an in-memory collection of all 
    /// objects of that type. Set up access through a well-known global 
    /// interface. Provide methods to add and remove objects...  
    /// 
    /// Provide  methods that select objects based on some criteria and 
    /// return fully instantiated objects or collections of objects whose 
    /// attribute values meet the criteria... 
    /// 
    /// Provide repositories only for aggregates... 
    /// [Evans, p. 151]
    /// </summary>
    public interface IRepository { }
}
