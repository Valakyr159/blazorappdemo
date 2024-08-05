using System.Text.Json;

namespace blazorappdemo;

public class CategoryService : ICategoryService
{
  private readonly HttpClient client;

  private readonly JsonSerializerOptions options;

  public CategoryService(HttpClient client)
  {
    this.client = client;
    options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
  }

  public async Task<List<Category>?> Get()
  {
    var response = await client.GetAsync("/v1/categories");
    var content = await response.Content.ReadAsStringAsync();
    if (!response.IsSuccessStatusCode)
    {
      throw new ApplicationException(content);
    }
    return await JsonSerializer.DeserializeAsync<List<Category>>(await response.Content.ReadAsStreamAsync());
  }

  public async Task<List<Category>?> GetCategorysAsync()
  {
    try
    {
      var response = await client.GetAsync("/v1/categories");
      response.EnsureSuccessStatusCode();
      using var stream = await response.Content.ReadAsStreamAsync();
      return await JsonSerializer.DeserializeAsync<List<Category>>(stream, options);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error al obtener la categoria: {ex.Message}");
      return null;
    }
  }

  public async Task<Category?> GetCategoryByIdAsync(int categoryId)
  {
    try
    {
      var response = await client.GetAsync($"/v1/categories/{categoryId}");
      response.EnsureSuccessStatusCode();
      using var stream = await response.Content.ReadAsStreamAsync();
      return await JsonSerializer.DeserializeAsync<Category>(stream, options);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error al obtener la categoria con ID {categoryId}: {ex.Message}");
      return null;
    }
  }

  public async Task<bool> CreateCategoryAsync(Category category)
  {
    try
    {
      var jsonContent = new StringContent(JsonSerializer.Serialize(category), System.Text.Encoding.UTF8, "application/json");
      var response = await client.PostAsync("/v1/categories", jsonContent);
      response.EnsureSuccessStatusCode();
      return true;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error al crear la categoria: {ex.Message}");
      return false;
    }
  }

  public async Task<bool> UpdateCategoryAsync(Category category)
  {
    try
    {
      var jsonContent = new StringContent(JsonSerializer.Serialize(category), System.Text.Encoding.UTF8, "application/json");
      var response = await client.PutAsync($"/v1/categories/{category.Id}", jsonContent);
      response.EnsureSuccessStatusCode();
      return true;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error al actualizar la categoria con ID {category.Id}: {ex.Message}");
      return false;
    }
  }

  public async Task<bool> DeleteCategoryAsync(int categoryId)
  {
    try
    {
      var response = await client.DeleteAsync($"/v1/categories/{categoryId}");
      response.EnsureSuccessStatusCode();
      return true;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error al eliminar la categoria con ID {categoryId}: {ex.Message}");
      return false;
    }
  }
}

public interface ICategoryService
{
  Task<List<Category>?> Get();
  Task<List<Category>?> GetCategorysAsync();
  Task<Category?> GetCategoryByIdAsync(int categoryId);
  Task<bool> CreateCategoryAsync(Category category);
  Task<bool> UpdateCategoryAsync(Category category);
  Task<bool> DeleteCategoryAsync(int categoryId);
}