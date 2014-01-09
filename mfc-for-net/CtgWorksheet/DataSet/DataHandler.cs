using CtgWorksheet.Model;
using MVCEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine;
using CtgWorksheet.Model.Attributes;
using MVCEngine.Model.Interface;
using System.Data;

namespace CtgWorksheet.DataSet
{
    public class DataHandler : IDisposable 
    {
        private CtgWorksheet _dataSet;

        public DataHandler()
        {
            _dataSet = new CtgWorksheet();
        }

        public void FillDataSet()
        {
            CtgWorksheet.GP_WORKSHEETRow row = _dataSet.GP_WORKSHEET.NewGP_WORKSHEETRow();
            row.GP_WKS_RECID = 0;
            row.GP_WKS_DESCRIPTION = "Screening(0)";
            row.GP_WKS_NAME = "Worksheet";
            _dataSet.GP_WORKSHEET.Rows.Add(row);

            CtgWorksheet.GP_PROBERow probe = _dataSet.GP_PROBE.NewGP_PROBERow();
            probe.GP_PR_RECID = 0;
            probe.GP_PR_CODE = "PROBE1";
            probe.GP_PR_NAME = "Probe one";
            _dataSet.GP_PROBE.Rows.Add(probe);

            probe = _dataSet.GP_PROBE.NewGP_PROBERow();
            probe.GP_PR_RECID = 1;
            probe.GP_PR_CODE = "PROBE2";
            probe.GP_PR_NAME = "Probe two";
            _dataSet.GP_PROBE.Rows.Add(probe);

            _dataSet.AcceptChanges();
        }

        public void FillContext(WorksheetContext ctx)
        {
            ctx.Context.Entites.ForEach((e) =>
            {
                Attribute attribute = e.Attributes.FirstOrDefault(a => a.IsTypeOf<Table>());
                if (attribute.IsNotNull())
                {
                    Table table = attribute.CastToType<Table>();
                    if (_dataSet.Tables.Contains(table.TableName))
                    {
                        IEntityCollection collection = e.Entities as IEntityCollection;
                        if (collection.IsNotNull())
                        {
                            foreach(DataRow row in _dataSet.Tables[table.TableName].Rows)
                            {
                                Entity entity = collection.CreateInstance(e.EntityType, true, new object[] { row }) as Entity;
                                if (entity.IsNotNull())
                                {
                                    collection.Add(entity);
                                }
                            }
                        }
                    }
                }
            });
            ctx.AcceptChanges();
        }

        public void EntityCreated(Entity e)
        {
            EntityRow entity = e.CastToType<EntityRow>();
            if (entity.IsNotNull() && entity.Row.IsNull())
            {
                if (entity.Table.IsNotNull())
                {
                    if (_dataSet.Tables.Contains(entity.Table.TableName))
                    {
                        DataRow row = _dataSet.Tables[entity.Table.TableName].NewRow();
                        _dataSet.Tables[entity.Table.TableName].Rows.Add(row);
                        entity.Row = row;
                    }
                }
            }
        }

        ~DataHandler()
        {
            Dispose();
        }

        public void Dispose()
        {
            _dataSet = null;
        }
    }
}
