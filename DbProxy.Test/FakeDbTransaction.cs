using System;
using System.Data;
using System.Data.Common;

namespace DbProxy.Test
{
    public class FakeDbTransaction : DbTransaction
    {
        public override IsolationLevel IsolationLevel => throw new NotImplementedException();

        protected override DbConnection DbConnection => throw new NotImplementedException();

        public override void Commit()
        {
            throw new NotImplementedException();
        }

        public override void Rollback()
        {
            throw new NotImplementedException();
        }
    }
}
