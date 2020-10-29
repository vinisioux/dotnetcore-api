using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Data.Context;
using Api.Domain.Entities;
using Api.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Repository
{
  public class BaseRepository<T> : IRepository<T> where T : BaseEntity
  {

    protected readonly MyContext _context;
    private DbSet<T> _dataset;

    public BaseRepository(MyContext context)
    {
      _context = context;
      _dataset = _context.Set<T>();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
      try
      {
        var user = await _dataset.SingleOrDefaultAsync(user => user.Id.Equals(id));

        if (user == null)
          return false;

        _dataset.Remove(user);
        await _context.SaveChangesAsync();
        return true;
      }
      catch (Exception err)
      {
        throw err;
      }
    }

    public async Task<T> InsertAsync(T item)
    {
      try
      {
        if (item.Id == Guid.Empty)
        {
          item.Id = Guid.NewGuid();
        }

        item.CreatedAt = DateTime.UtcNow;

        _dataset.Add(item);

        await _context.SaveChangesAsync();
      }
      catch (Exception err)
      {
        throw err;
      }
      return item;
    }

    public async Task<bool> ExistAsync(Guid id)
    {
      return await _dataset.AnyAsync(p => p.Id.Equals(id));
    }

    public async Task<T> SelectAsync(Guid id)
    {
      try
      {
        return await _dataset.SingleOrDefaultAsync(user => user.Id.Equals(id));
      }
      catch (Exception err)
      {
        throw err;
      }
    }

    public async Task<IEnumerable<T>> SelectAsync()
    {
      try
      {
        return await _dataset.ToListAsync();
      }
      catch (Exception err)
      {
        throw err;
      }
    }

    public async Task<T> UpdateAsync(T item)
    {
      try
      {
        var user = await _dataset.SingleOrDefaultAsync(user => user.Id.Equals(item.Id));

        if (user == null)
          return null;

        item.UpdatedAt = DateTime.Now;
        item.CreatedAt = user.CreatedAt;

        _context.Entry(user).CurrentValues.SetValues(item);
        await _context.SaveChangesAsync();
      }
      catch (Exception err)
      {
        throw err;
      }
      return item;
    }
  }
}