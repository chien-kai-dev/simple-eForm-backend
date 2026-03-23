using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using axiosTest.Models;
using axiosTest.Repositories;

namespace axiosTest.Repositories
{
	public class FormInfoRepository
	{
    	private readonly OracleRepository _repo;
	
    	public FormInfoRepository(OracleRepository repo)
    	{
        	_repo = repo;
    	}
	
    	// Create
    	public async Task<int> InsertAsync(FormInfo formInfoObj)
    	{
        	string sql = @"
            	INSERT INTO ""FormInfo"" (""FormNum"", ""CreatedDate"", ""CreatedTime"", ""Creator"")
            	VALUES (:FormNum, TO_DATE(:CreatedDate, 'YYYY-MM-DD'), TO_DSINTERVAL(:CreatedTime), :Creator)
        	";
        	
        	var parameters = new 
        	{
            	FormNum = formInfoObj.FormNum, 
            	CreatedDate = formInfoObj.CreatedDate.ToString("yyyy-MM-dd"), 
            	CreatedTime = $"0 {formInfoObj.CreatedTime:hh\\:mm\\:ss}", 
            	Creator = formInfoObj.Creator
        	};
        	
        	// formInfoObj.CreatedTime
        	// .ToString(@"dd\.hh\:mm\:ss")
        	
        	return await _repo.ExecuteAsync(sql, parameters);
    	}
	
    	// Read
    	public async Task<IEnumerable<FormInfo>> GetAllAsync()
    	{
        	string sql = "select * from \"FormInfo\"";
        	return await _repo.QueryAsync<FormInfo>(sql);
    	}
	
    	// Read Single
    	/*
    	public async Task<FormInfo> GetByIdAsync(decimal id)
    	{
        	string sql = "SELECT ID, NAME, ADDRESS FROM EMPLOYEE WHERE ID = :Id";
        	return await _repo.QuerySingleOrDefaultAsync<FormInfo>(sql, new { Id = id });
    	}
    	*/
	
    	// Update
    	/*
    	public async Task<int> UpdateAsync(FormInfo formInfoObj)
    	{
        	string sql = @"
            	UPDATE EMPLOYEE
            	SET NAME = :Name,
                	ADDRESS = :Address
            	WHERE ID = :Id
        	";
        	
        	var parameters = new
        	{
            	Name = formInfoObj.Name,
            	Address = formInfoObj.Address,
            	Id = formInfoObj.Id
        	};
        	return await _repo.ExecuteAsync(sql, parameters);
    	}
    	*/
	
    	// Delete
    	/*
    	public async Task<int> DeleteAsync(decimal id)
    	{
        	string sql = "DELETE FROM EMPLOYEE WHERE ID = :Id";
        	return await _repo.ExecuteAsync(sql, new { Id = id });
    	}
    	*/
	}
}
