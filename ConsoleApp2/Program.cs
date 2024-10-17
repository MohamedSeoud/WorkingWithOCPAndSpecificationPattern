using System.Linq.Expressions;

Console.WriteLine("itemToPrint");

Specification<GameModel> spec = new Specification<GameModel>(
    game => game.Id == 1);
 GameRepository repo = new GameRepository();
var allGames = repo.GetAll();
var itemToPrint = repo.GetAllWithSpec(spec);
Console.WriteLine(itemToPrint.FirstOrDefault()?.Name ??"");

GameSpec gameSpec = new GameSpec(allGames[2]);
GameFilter gameFilter = new GameFilter();
foreach(GameModel game in gameFilter.Filter(allGames, gameSpec))
{
    Console.WriteLine(game.Name);
}
Console.ReadKey();


public class ProductFilter
{
    public IEnumerable<GameModel> FilterByName(IEnumerable<GameModel> arrToFilter, string target)
    {
        foreach(GameModel model in arrToFilter)
            if(model.Name == target) yield return model;
    }
    
}
public interface ISpecification<TEntity> where TEntity : class
{
    public bool IsSatisfied(TEntity entity);
}
public interface IFilter<TEntity> where TEntity : class
{
    public IEnumerable<TEntity> Filter(IEnumerable<TEntity> entities, ISpecification<TEntity> specs);
}
public class GameSpec : ISpecification<GameModel>
{
    private readonly GameModel _game;
    public GameSpec(GameModel game)
    {
        _game = game;
    }
    public bool IsSatisfied(GameModel entity)
    {
        return _game.Name == entity.Name;
    }
}
public class GameFilter : IFilter<GameModel>
{

    public IEnumerable<GameModel> Filter(IEnumerable<GameModel> entities, ISpecification<GameModel> specs)
    {
        foreach(GameModel entitiy in entities)
            if(specs.IsSatisfied(entitiy)) yield return entitiy;
    }

}
public class GameRepository : IGameRepository
{
    private readonly SeedData _seedData;
    public GameRepository()
    {
        _seedData = new SeedData([
            new GameModel{
                Id = 1,
                Name ="Test",
            },
            new GameModel{
                Id = 2,
                Name ="Test2",
            },
            new GameModel{
                Id = 3,
                Name ="Test3",
            },
            new GameModel{
                Id = 4,
                Name ="Test4",
            },
            new GameModel{
                Id = 5,
                Name ="Test5",
            }
            ]);
    }
    public List<GameModel> GetAllWithSpec(Specification<GameModel> specification)
    {
        return SpecificationQueryBuilder.GetQuery(
            _seedData.GameList.AsQueryable(), specification
            ).ToList();
    }
    public List<GameModel> GetAll()
    {
        return _seedData.GameList.ToList();
    }

}
public interface IGameRepository
{
    public List<GameModel> GetAllWithSpec(Specification<GameModel> specification);
    public List<GameModel> GetAll();

}
public static class SpecificationQueryBuilder
{
    public static IQueryable<TEntity> GetQuery<TEntity>(
        IQueryable<TEntity> inputQuery,
        Specification<TEntity> specification)
        where TEntity : class
    {
        var query = inputQuery.AsQueryable();
        if (specification?.Criteria != null) 
        {
            query = query.Where(specification.Criteria);
        }
        return query;

    }
}
public class Specification<TEntity> where TEntity : class
{
    public Specification()
    {
        
    }
    public Expression<Func<TEntity, bool>>? Criteria { get;}
    public Specification(Expression<Func<TEntity,bool>> criteria)
    {
        Criteria = criteria;
    }
}
public class GameModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
}
public class SeedData 
{
    public List<GameModel> GameList { get; }
    public SeedData(List<GameModel> gameList)
    {
        GameList = gameList;
    }
}


