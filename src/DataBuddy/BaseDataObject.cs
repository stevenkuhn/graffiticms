using System;
using System.Collections.Generic;

namespace DataBuddy
{
	
	public abstract class BaseDataObject
	{
	
		#region Private Members
	
		private bool _isNew = true;
		private bool _isDirty = false;
	    private bool _isLoaded = false;
		#endregion
	
		#region Protected
	
		/// <summary>
		/// Marks the object as Dirty
		/// </summary>
		protected void MarkDirty()
		{
			_isDirty = true;
		}
	
		/// <summary>
		/// Marks an object as New. Should be called when object does not 
		/// exist in the datastore
		/// </summary>
		protected void MarkNew()
		{
			_isDirty = true;
			_isNew = true;
		}
	
	
		/// <summary>
		/// After an object has been saved, ResetStatus will mark the object
		/// as not being dirty or new
		/// </summary>
		protected void ResetStatus()
		{
			_isDirty = false;
			_isNew = false;
		}
		#endregion
	
		#region Public

        protected virtual void BeforeValidate()
        {
            
        }
	
		protected virtual void BeforeInsert()
		{
		}
	
		protected virtual void BeforeUpdate()
		{
		}

        protected virtual void AfterInsert()
        {
            
        }

        protected virtual void AfterUpdate()
        {
            
        }

        protected  virtual  void AfterCommit()
        {
            
        }

        protected virtual void BeforeRemove(bool isDestroy)
        {
            
        }

        protected virtual void AfterRemove(bool isDestroy)
        {

        }

        protected virtual void Loaded()
        {
            _isLoaded = true;
        }
	
		#region Save

	    protected abstract List<Parameter> GetParameters();
	    protected abstract void SetPrimaryKey(int pkValue);
	    protected abstract Table GetTable();

        protected virtual List<Parameter> FormatParameters(string username, DateTime dt)
        {
            List<Parameter> parameters = GetParameters();
            foreach(Parameter p in parameters)
            {
                if(p.Name == "CreatedBy" || p.Name == "ModifiedBy")
                {
                    if (p.Value == null && username != null)
                        p.Value = username;
                }
                else if (IsNew && p.Name == "CreatedOn")
                {
                        p.Value = dt;
                }
                else if (p.Name == "ModifiedOn")
                {
                    p.Value = dt;
                }
            }

            return parameters;
        }

        public virtual void Save()
        {
            Save(null, DateTime.Now);
        }

        public virtual void Save(string username)
        {
            Save(username, DateTime.Now);
        }

        public virtual void Save(DateTime modifiedTime)
        {
            Save(null, DateTime.Now);
        }

		/// <summary>
		/// Commits any changes to the datastore. Although it can be overriden, derrived classes should call base.Save();
		/// </summary>
        public virtual void Save(string username, DateTime modifiedTime)
		{
		    BeforeValidate();


		    if (IsNew)
		    {
		        BeforeInsert();
		        SetPrimaryKey(DataService.Insert(GetTable(), FormatParameters(username, modifiedTime)));
		    }
		    else
		    {
		        BeforeUpdate();
		        DataService.Update(GetTable(), FormatParameters(username, modifiedTime));
		    }

            if(IsNew)
                AfterInsert();
            else
                AfterUpdate();

		    //We should no longer be new or dirty
		    ResetStatus();


		    AfterCommit();
		}

	    #endregion
	
		#region Delete
		
        ///// <summary>
        ///// Marks the object for deletion and then calls Save to persist the change
        ///// </summary>
        //public void Destroy()
        //{
        //}		
		
        ///// <summary>
        ///// Delete marks an object as ready to be deleted, but does not actually remove the object.
        ///// You must call Save to actually remove the object. Changes to the delete process should be made
        ///// to the protected method, CommitDelete which is called from Save if the object has been marked for
        ///// deletion.
        ///// </summary>
        //public void Delete()
        //{
        //}
		#endregion
	
		#region State
	
	
		/// <summary>
		/// Returns true if this object has been marked as new.
		/// </summary>
		public bool IsNew
		{
			get { return _isNew; }
		}
	
		/// <summary>
		/// Returns true if this object has been marked as dirty (property has changed or the object is new)
		/// </summary>
		public virtual bool IsDirty
		{
			get { return _isDirty; }
		}

	    public bool IsLoaded
	    {
            get { return _isLoaded; }
	    }

		#endregion
	
		#endregion
	}
}


