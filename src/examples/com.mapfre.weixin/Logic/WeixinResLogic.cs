using System;
using System.Collections.Generic;
using JR.DevFw.Data;
using JR.DevFw.Data.Extensions;
using Com.Plugin.Core;
using Com.Plugin.Entity;
using Com.Plugin.ILogic;

namespace Com.Plugin.Logic
{
    public class WeixinResLogic : IWeixinResLogic
    {
        private DbGenerator _dao;

        public WeixinResLogic(DbGenerator dao)
        {
            this._dao = dao;
        }

        public IWxRes GetResById(int id)
        {
            IWxRes res = null;
            DataBaseAccess db = this._dao.New();
            db.ExecuteReader("SELECT Id ,ResKey,TypeId,TypeName,CreateTime,UpdateTime FROM wx_res WHERE Id=" + id.ToString(), rd =>
            {
                if (rd.Read())
                {
                    int typeId = rd.GetInt32(2);
                    if (typeId == 1)
                    {
                        res = new TextRes
                        {
                            CreateTime = rd.GetDateTime(4),
                            UpdateTime = rd.GetDateTime(5),
                            TypeId = typeId,
                            TypeName = rd.GetString(3),
                            ResKey = rd.GetString(1),
                            Id = rd.GetInt32(0)
                        };
                    }
                    else
                    {
                        res = new ArticleRes
                        {
                            CreateTime = rd.GetDateTime(4),
                            UpdateTime = rd.GetDateTime(5),
                            TypeId = typeId,
                            ResKey = rd.GetString(1),
                            TypeName = rd.GetString(3),
                            Id = rd.GetInt32(0)
                        };
                    }
                }
            });

            if (res != null)
            {
                TextRes tRes;
                ArticleRes atRes;

                if ((tRes = (res as TextRes)) != null)
                {
                    db.ExecuteReader("SELECT Content FROM wx_text Where resid=" + tRes.Id.ToString(), rd =>
                    {
                        if (rd.Read())
                        {
                            tRes.Content = rd.GetString(0);
                        }
                    });
                }
                else
                {
                    atRes = res as ArticleRes;

                    db.ExecuteReader("SELECT * FROM wx_art_item WHERE resid=" + atRes.Id.ToString() + " ORDER BY sort,id", rd =>
                    {
                        if (rd.HasRows)
                        {
                            atRes.Items = rd.ToEntityList<ArticleResItem>();
                        }

                    });

                    if (atRes.Items != null)
                    {
                        foreach (var articleResItem in atRes.Items)
                        {
                            articleResItem.SetArticle(atRes);
                        }
                    }
                    else
                    {
                        atRes.Items = new List<ArticleResItem>();
                    }
                }
            }

            return res;
        }


        public bool CheckResKey(int id, string resKey)
        {
            object result = this._dao.New().ExecuteScalar(new SqlQuery(
                 "SELECT count(0) FROM wx_res WHERE resKey =@resKey AND ( id<>@id OR @id=0)",
                     new object[,]
                    {
                        {"@resKey",resKey},
                        {"@id",id}
                    }));

            return Convert.ToInt32(result) == 0;
        }


        public int Save(IWxRes res)
        {
            TextRes textRes = res as TextRes;
            if (textRes != null)
            {
                return SaveTextRes(textRes);
            }
            else
            {
                ArticleRes artRes = res as ArticleRes;
                return this.SaveArticleRes(artRes);
                
            }
        }


        private int SaveArticleRes(ArticleRes artRes)
        {
            DataBaseAccess db = this._dao.New();
            var data = new object[,]
            {
                {"@Id", artRes.Id},
                {"@ResKey", artRes.ResKey},
                {"@CreateTime", artRes.CreateTime},
                {"@UpdateTime", artRes.UpdateTime},
                {"@TypeId", artRes.TypeId},
                {"@TypeName", artRes.TypeName}
            };

            if (artRes.Id > 0)
            {
                db.ExecuteNonQuery(new SqlQuery("UPDATE wx_res SET reskey=@resKey,updatetime=@updatetime WHERE id=@Id", data));
            }
            else
            {
                int row = db.ExecuteNonQuery(new SqlQuery(@"INSERT INTO wx_res (ResKey,TypeId,TypeName,CreateTime,UpdateTime)
                            VALUES (@ResKey,@TypeId,@TypeName,@CreateTime,@UpdateTime)", data));
                if (row == 1)
                {
                    int id = Convert.ToInt32(db.ExecuteScalar("SELECT MAX(id) FROM wx_res"));
                    return id;
                }
            }

            return artRes.Id;
        }

        private int SaveTextRes(TextRes textRes)
        {
            DataBaseAccess db = this._dao.New();
            var data = new object[,]
            {
                {"@Id", textRes.Id},
                {"@ResKey", textRes.ResKey},
                {"@CreateTime", textRes.CreateTime},
                {"@UpdateTime", textRes.UpdateTime},
                {"@TypeId", textRes.TypeId},
                {"@TypeName", textRes.TypeName},
                {"@Content", textRes.Content}
            };

            if (textRes.Id > 0)
            {
                db.ExecuteNonQuery(new SqlQuery("UPDATE wx_res SET reskey=@resKey,updatetime=@updatetime WHERE id=@Id", data),
                    new SqlQuery("Update wx_text Set wx_text.content=@Content where resId=@Id", data));
            }
            else
            {
                int row = db.ExecuteNonQuery(new SqlQuery(@"INSERT INTO wx_res (ResKey,TypeId,TypeName,CreateTime,UpdateTime)
                            VALUES (@ResKey,@TypeId,@TypeName,@CreateTime,@UpdateTime)", data));
                if (row == 1)
                {
                    int id = Convert.ToInt32(db.ExecuteScalar("SELECT MAX(id) FROM wx_res"));
                    data[0, 1] = id;
                    db.ExecuteNonQuery(new SqlQuery(@"INSERT INTO wx_text(ResId,Content)VALUES(@Id,@Content)", data));
                    return id;
                }
            }

            return textRes.Id;
        }

        public ArticleResItem GetArticleItem(int id)
        {
            ArticleResItem res = null;
            DataBaseAccess db = this._dao.New();
            db.ExecuteReader(
                "SELECT Id ,ResId,Title ,Content,Url ,Pic ,Sort,Enabled,Description FROM wx_art_item WHERE Id=" + id.ToString(),
                rd =>
                {
                    if (rd.Read())
                    {
                        int resId = rd.GetInt32(1);
                        res = new ArticleResItem(new ArticleRes { Id = resId });
                        res.Id = rd.GetInt32(0);
                        res.Title = rd.GetString(2);
                        res.Content = rd.GetString(3);
                        res.Url = rd.GetString(4);
                        res.Pic = rd.GetString(5);
                        res.Sort = rd.GetInt32(6);
                        res.Enabled = rd.GetBoolean(7);
                        res.Description = rd.GetString(8);
                    }
                });
            return res;
        }

        public int SaveArticleItem(int id, ArticleResItem item)
        {
            DataBaseAccess db = this._dao.New();
            var data = new object[,]
            {
                {"@ResId", id},
                {"@Description",item.Description},
                {"@Content", item.Content},
                {"@Enabled", item.Enabled},
                {"@Pic", item.Pic},
                {"@Id", item.Id},
                {"@Sort", item.Sort},
                {"@Title", item.Title},
                {"@Url", item.Url},
            };

            if (item.Id > 0)
            {
                db.ExecuteNonQuery(new SqlQuery(@"UPDATE wx_art_item SET Title = @Title ,
                    [Content]=@Content, Description=@Description,[Url] = @Url ,[Pic] = @Pic ,[Sort] = @Sort ,[Enabled] = @Enabled
                    WHERE [ResId] = @ResId AND id=@Id",
                    data));
            }
            else
            {
                int row =
                    db.ExecuteNonQuery(new SqlQuery(@"INSERT INTO wx_art_item ([ResId],[Title],Description,[Content],[Url],[Pic],[Sort],[Enabled])
                        VALUES (@ResId,@Title,@Description,@Content,@Url,@Pic,@Sort,@Enabled)", data));
                if (row == 1)
                {
                    return Convert.ToInt32(db.ExecuteScalar("SELECT MAX(id) FROM wx_art_item"));
                }
            }

            return item.Id;
        }


        public IWxRes GetResByKey(string resKey)
        {
            IWxRes res = null;
            DataBaseAccess db = this._dao.New();
            db.ExecuteReader(new SqlQuery("SELECT Id ,ResKey,TypeId,TypeName,CreateTime,UpdateTime FROM wx_res WHERE ResKey=@ResKey",
                new object[,]
            {
                {"@ResKey",resKey}
            }),rd =>
            {
                if (rd.Read())
                {
                    int typeId = rd.GetInt32(2);
                    if (typeId == 1)
                    {
                        res = new TextRes
                        {
                            CreateTime = rd.GetDateTime(4),
                            UpdateTime = rd.GetDateTime(5),
                            TypeId = typeId,
                            TypeName = rd.GetString(3),
                            ResKey = rd.GetString(1),
                            Id = rd.GetInt32(0)
                        };
                    }
                    else
                    {
                        res = new ArticleRes
                        {
                            CreateTime = rd.GetDateTime(4),
                            UpdateTime = rd.GetDateTime(5),
                            TypeId = typeId,
                            ResKey = rd.GetString(1),
                            TypeName = rd.GetString(3),
                            Id = rd.GetInt32(0)
                        };
                    }
                }
            });

            if (res != null)
            {
                TextRes tRes;
                ArticleRes atRes;

                if ((tRes = (res as TextRes)) != null)
                {
                    db.ExecuteReader("SELECT Content FROM wx_text Where resid=" + tRes.Id.ToString(), rd =>
                    {
                        if (rd.Read())
                        {
                            tRes.Content = rd.GetString(0);
                        }
                    });
                }
                else
                {
                    atRes = res as ArticleRes;

                    db.ExecuteReader("SELECT * FROM wx_art_item WHERE resid=" + atRes.Id.ToString()+" ORDER BY sort,id", rd =>
                    {
                        if (rd.HasRows)
                        {
                            atRes.Items = rd.ToEntityList<ArticleResItem>();
                        }

                    });

                    if (atRes.Items != null)
                    {
                        foreach (var articleResItem in atRes.Items)
                        {
                            articleResItem.SetArticle(atRes);
                        }
                    }
                    else
                    {
                        atRes.Items = new List<ArticleResItem>();
                    }
                }
            }

            return res;
        }

        public void DeleteRes(int resId)
        {
            DataBaseAccess db = this._dao.New();
            IWxRes res = this.GetResById(resId);
            object[,] data = new object[,]
            {
                {"@resId", resId}
            };

            if (res.Type() == 1)
            {
                db.ExecuteNonQuery(
                    new SqlQuery("DELETE FROM wx_res WHERE id=@resId", data),
                    new SqlQuery("DELETE FROM wx_text Where resid=@resId", data)
                    );
            }
            else
            {
                db.ExecuteNonQuery(
                    new SqlQuery("DELETE FROM wx_res WHERE id=@resId", data),
                    new SqlQuery("DELETE FROM wx_art_item Where resid=@resId", data)
                    );
            }
        }
    }
}
