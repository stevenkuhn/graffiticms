using System;
using System.Collections.Generic;
using System.Text;

namespace Graffiti.Core
{
    public partial class ObjectStore
    {
        protected override void BeforeValidate()
        {
            base.BeforeValidate();

            if (IsNew && UniqueId == Guid.Empty)
                UniqueId = Guid.NewGuid();

            if (string.IsNullOrEmpty(Type))
                throw new Exception("ObjectStore.Type must be defined");

            if (string.IsNullOrEmpty("Name"))
                throw new Exception("ObjectStore.Name must be defined");

        }
    }
}
