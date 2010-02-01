using System;
using System.Collections.Generic;
using System.Text;

namespace Graffiti.Core
{
    public partial class DataBuddyBase
    {
        protected override void AfterCommit()
        {

            Events.Instance().ExecuteAfterCommitEvent(this);
            base.AfterCommit();
        }

        protected override void BeforeValidate()
        {
            Events.Instance().ExecuteBeforeValidateEvent(this);
            base.BeforeValidate();
        }

        protected override void BeforeInsert()
        {
            Events.Instance().ExecuteBeforeInsertEvent(this);
            base.BeforeInsert();
        }

        protected override void BeforeUpdate()
        {
            Events.Instance().ExecuteBeforeUpdateEvent(this);
            base.BeforeUpdate();
        }

        protected override void AfterInsert()
        {
            Events.Instance().ExecuteAfterInsertEvent(this);
            base.AfterInsert();
        }

        protected override void AfterUpdate()
        {
            Events.Instance().ExecuteAfterUpdateEvent(this);
            base.AfterUpdate();
        }

        protected override void BeforeRemove(bool isDestroy)
        {
            if (isDestroy)
                Events.Instance().ExecuteBeforeDestroyEvent(this);
            else
                Events.Instance().ExecuteBeforeRemoveEvent(this);

            base.BeforeRemove(isDestroy);
        }

        protected override void AfterRemove(bool isDestroy)
        {
            if (isDestroy)
                Events.Instance().ExecuteAfterDestroyEvent(this);
            else
                Events.Instance().ExecuteAfterRemoveEvent(this);

            base.AfterRemove(isDestroy);
        }

    }
}
