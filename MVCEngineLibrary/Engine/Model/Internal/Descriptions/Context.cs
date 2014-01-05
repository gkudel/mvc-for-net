using MVCEngine.Model.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MVCEngine;
using MVCEngine.Model.Interface;
using System.Diagnostics;

namespace MVCEngine.Model.Internal.Descriptions
{
    public class Context : IDisposable
    {
        #region Member
        private bool _isModified;
        #endregion Member

        #region Constructor
        internal Context()
        {
            Entites = new List<EntityClass>();
            Relations = new List<EntitiesRelation>();
            IsModified = false;
        }
        #endregion Constructor

        #region Properties
        public string Name { get; internal set; }        
        public List<EntityClass> Entites { get; internal set; }
        internal Type EntitiesContextType { get; set; }
        internal List<EntitiesRelation> Relations { get; set; }
        internal Action ContextModifed { get; set; }
        
        public bool IsModified
        {
            get
            {
                return _isModified;
            }
            internal set
            {
                _isModified = value;
                if (_isModified && ContextModifed.IsNotNull())
                {
                    ContextModifed();
                }
            }
        }

        public Action<Entity> EntityCreated { get; set; }
        public Action<Entity> EntityInitialized { get; set; }
        #endregion Properties

        #region Copy
        internal Context Copy()
        {
            Context ctx = new Context() 
            {
                Name = this.Name
            };
            Entites.ForEach((t) =>
            {
                EntityClass entity = new EntityClass()
                {
                    Name = t.Name,
                    EntityType = t.EntityType,
                    PrimaryKey = t.PrimaryKey,
                };
                entity.Validators.AddRange(t.Validators);
                entity.Attributes.AddRange(t.Attributes);
                t.SynchronizedCollection.Keys.ToList().ForEach((s) =>
                {
                    entity.SynchronizedCollection.Add(s, new List<string>(t.SynchronizedCollection[s]));
                });
                ctx.Entites.Add(entity);
            });
            Relations.ForEach((r) => 
            {
                EntitiesRelation relation = new EntitiesRelation() 
                {
                    Ordinal = r.Ordinal,
                    Name = r.Name,
                    ParentEntityName = r.ParentEntityName,                    
                    ChildEntityName = r.ChildEntityName,
                    ParentKey = r.ParentKey,
                    ParentType = r.ParentType, 
                    ParentValue = r.ParentValue,
                    ChildKey = r.ChildKey, 
                    ChildType = r.ChildType, 
                    ChildValue = r.ChildValue,
                    OnDelete = r.OnDelete
                };
                ctx.Relations.Add(relation);
                relation.ParentEntity = ctx.Entites.FirstOrDefault(t => t.Name == r.ParentEntityName);
                relation.ChildEntity = ctx.Entites.FirstOrDefault(t => t.Name == r.ChildEntityName);
            });
            ctx.Entites.ForEach((t) =>
            {
                EntityClass entity = Entites.FirstOrDefault(e => e.Name == t.Name);
                Debug.Assert(entity.IsNotNull(), "Somthing gone wrrong");
                entity.Properties.ForEach((p) =>
                {
                    EntityProperty property = new EntityProperty()
                    {
                        Name = p.Name,
                        PropertyType = p.PropertyType,
                        PrimaryKey = p.PrimaryKey,
                        DefaultValue = p.DefaultValue,
                        Setter = p.Setter,
                        Getter = p.Getter
                    };
                    if (p.ReletedEntity.IsNotNull())
                    {
                        property.ReletedEntity = new ReletedEntity()
                        {
                            Related = p.ReletedEntity.Related,
                            RelatedEntity = ctx.Entites.FirstOrDefault(e => e.Name == p.ReletedEntity.RelatedEntityName),
                            RelatedEntityName = p.ReletedEntity.RelatedEntityName,
                            Relation = ctx.Relations.FirstOrDefault(r => r.Ordinal == p.ReletedEntity.Relation.Ordinal),
                            RelationName = p.ReletedEntity.RelationName
                        };
                        property.ReletedEntity.Discriminators.AddRange(p.ReletedEntity.Discriminators);
                    }
                    property.Validators.AddRange(p.Validators);
                    property.Attibutes.AddRange(p.Attibutes);
                    p.Formatters.Keys.ToList().ForEach((k) => property.Formatters.Add(k, p.Formatters[k]));
                    t.Properties.Add(property);
                });
                if (entity.PrimaryKeyProperty.IsNotNull()) t.PrimaryKeyProperty = t.Properties.FirstOrDefault(p => p.Name == entity.PrimaryKeyProperty.Name);
                if (entity.DynamicProperties.IsNotNull())
                {
                    t.DynamicProperties = new DynamicProperties()
                    {
                        CodeProperty = entity.DynamicProperties.CodeProperty,
                        Property = t.Properties.FirstOrDefault(p => p.Name == entity.DynamicProperties.Property.Name)
                    };
                    entity.DynamicProperties.ValuesProperties.Keys.ToList().ForEach((type) =>
                    {
                        t.DynamicProperties.ValuesProperties.Add(type, entity.DynamicProperties.ValuesProperties[type]);
                    });
                }
            });
            return ctx;
        }
        #endregion Copy

        #region Initialize 
        internal Context InitailizeRows(EntitiesContext mctx, Dictionary<string, Func<object, object>> ini)
        {
            Entites.ForEach((e) =>
            {
                e.Entities = ini[e.Name](mctx).CastToType<IEnumerable<Entity>>();
                IEntityCollection collection = e.Entities as IEntityCollection;
                if (collection.IsNotNull())
                {
                    collection.Context = this;
                }
            });
            return this;
        }
        #endregion Initialize

        #region Dispose
        public void Dispose()
        {
            Entites.ForEach((t) =>
            {
                t.Entities = null;
            });
            EntityCreated = null;
            EntityInitialized = null;
            ContextModifed = null;
        }

        ~Context()
        {
            Dispose();
        }
        #endregion Dispose
    }
}
