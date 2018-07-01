using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using SmartRecipes.Mobile.Infrastructure;

namespace SmartRecipes.Mobile.ReadModels
{
    public static class Repository
    {
        public static Monad.Reader<Enviroment, Task<TModel>> RetrievalAction<TModel, TResponse>(
            Func<ApiClient, Task<Option<TResponse>>> apiCall,
            Monad.Reader<Enviroment, Task<TModel>> databaseQuery,
            Func<TResponse, TModel> responseMapper,
            Func<TModel, IEnumerable<object>> envtabaseMapper)
        {
            return env => apiCall(env.Api).Bind(ro => ro.Match(
                r =>
                {
                    var model = responseMapper(r);
                    var newItems = envtabaseMapper(model);
                    var updateTask = newItems.Fold(Task.FromResult(Unit.Default), (t, i) => t.Bind(_ => env.Db.AddOrReplaceAsync(i)));
                    return updateTask.Map(_ => model);
                },
                () => databaseQuery(env)
            ));
        }
    }
}
