using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Service.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        [HttpGet]
        public ActionResult Get()
        {
            return Json(DAL.UserInfo.Instance.GetCount());
        }
        [HttpPost]
        public ActionResult Post([FromBody] Model.UserInfo users)
        {
            try
            {
                int n = DAL.UserInfo.Instance.Add(users);
                return Json(Result.Ok("添加成功"));
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("primary"))
                    return Json(Result.Err("用户名已存在"));
                else if (ex.Message.ToLower().Contains("null"))
                    return Json(Result.Err("用户名、密码、身份不能为空"));
                else
                    return Json(Result.Err(ex.Message));
            }
        }
        [HttpPost("check")]//管理员用户登录
        public ActionResult PUserCheck([FromBody] Model.UserInfo users)
        {
            try
            {
                var result = DAL.UserInfo.Instance.GetModel(users.userName);
                if (result == null)
                    return Json(Result.Err("用户名错误"));
                else if (result.passWord == users.passWord)
                {
                    if (result.type == "管理员")
                    {
                        result.passWord = "*********";
                        return Json(Result.Ok("管理员登录成功", result));
                    }
                    else
                        return Json(Result.Err("只有管理员能够进入后台管理"));
                }
                else
                    return Json(Result.Err("密码错误"));  
            }
            catch (Exception ex)
            {
                return Json(Result.Err(ex.Message));
            }
        }
        [HttpPost("getCheck")]//一般用户登录
        public ActionResult getUserCheck([FromBody] Model.UserInfo users)
        {
            try
            {
                var result = DAL.UserInfo.Instance.GetModel(users.userName);
                if (result == null)
                    return Json(Result.Err("用户名错误"));
                else if (result.passWord == users.passWord)
                {          
                        result.passWord = "*********";
                        return Json(Result.Ok("登录成功", result));
                }
                else
                    return Json(Result.Err("密码错误"));
            }
            catch (Exception ex)
            {
                return Json(Result.Err(ex.Message));
            }
        }
        [HttpPost("page")]//分页获取用户数据
        public ActionResult getPage([FromBody]Model.Page page)
        {
            var result = DAL.UserInfo.Instance.GetPage(page);
            if(result=Count()==0)
                return Json(Result.Err("返回记录数为0"));
            else
                return Json(Result.Ok(result));
        }

        [HttpPut]
        public ActionResult Put([FromBody]Model.UserInfo users)
        {
            try
            {
                var n = DAL.UserInfo.Instance.Update(users);
                if(n!=0)
                    return Json(Result.Ok("修改成功"));
                else
                    return Json(Result.Err("用户名不存在"));

            }
            catch(Exception ex)
            {
                if(ex.Message.ToLower().Contains("null"))
                    return Json(Result.Err("密码,身份不能为空"));
                else
                    return Json(Result.Err(ex.Message));
            }
        }
        [HttpDelete("{usename}")]
        public ActionResult Delete(string username)
        {
            try
            {
                var n = DAL.UserInfo.Instance.Delete(username);
                if (n != 0)
                    return Json(Result.Ok("删除成功"));
                else
                    return Json(Result.Err("用户名不存在"));

            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("foreign"))
                    return Json(Result.Err("发布了作品或活动的用户不能删除"));
                else
                    return Json(Result.Err(ex.Message));
            }
        }

    }
 }
