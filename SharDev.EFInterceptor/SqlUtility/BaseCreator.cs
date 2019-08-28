﻿using System.Text;

namespace SharDev.EFInterceptor.SqlUtility
{
    public abstract class BaseCreator
    {
        public StringBuilder _baseQueryBuilder;

        protected BaseCreator()
        {
            _baseQueryBuilder = new StringBuilder();
        }

        public string GetQuery() => _baseQueryBuilder.ToString();
    }
}
