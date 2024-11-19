# 定时任务工具

使用***Worker Service***技术结合Quartz定时任务框架, 打包后运行于宿主机(服务器)上作后台任务,宿主维护应用程序的生命周期。宿主还提供了一些常见的特性，如依赖注入、日志记录和配置。***Worker Service***是长时间运行的服务，执行一些规律发生的工作负载。
---
# 使用技术
.Net6、Quartz.Net、Log4.Net、SqlSugar
---

# 项目相关重要文件说明

├─Appsettings.cs  
├─appsettings.Development.json  
├─appsettings.json  
├─log4net.config		_______***log4net配置文件***  
├─Program.cs  
├─QuartzService.csproj  
├─QuartzService.csproj.user  
├─Tasks			_______***定时任务文件夹***  
|   ├─NewJob.cs  
├─Reflex		_______***反射相关工具包***  
|   ├─MapperUtils.cs  
|   ├─ReflectionUtils.cs  
|   └ValidateCondition.cs  
├─Properties  
|     ├─launchSettings.json  
|     ├─PublishProfiles  
|     |        ├─FolderProfile.pubxml  
|     |        └FolderProfile.pubxml.user  
├─Models		_______***模型层***  
|   ├─Config			
|   |   └TaskEntity.cs      _______***反射实体配置类***  
├─Extensions  
|     └QuartzConfiguratorExtension.cs  

**QuartzTask数组相关键值对说明**  
&emsp;&emsp;**name**：为自定义任务类名。  
&emsp;&emsp;**status**：true为启用,false关闭。  
&emsp;&emsp;**policy**：Current为程序启动立即执行,Cron为根据表达式执行。  
&emsp;&emsp;**cron**：cron表达式，只有**policy**的值为"**Cron**"时才会生效。  

**DBS数组相关键值对说明**  
&emsp;&emsp;**ConnId**:  数据库id,注入时使用, 区分不同库。  
&emsp;&emsp;**DbType**: 数据库类型, 1为SQL server数据库, 0为MySQL数据库，如果要添加支持的数据库，先查看SQL sugar是否支持目标数据库类型。   
&emsp;&emsp;**ConnectionString**: 数据库连接字符串。

**如何使用**：在Tasks文件夹中自定义任务类,实现IJob接口,在Execute实现任务即可；然后在appsettings.json配置文件中的QuatzTask数组下配置刚刚自定义的类。如果需要关闭相关任务,部署以后直接在配置文件中把相应任务的status设置为false即可。

---

## 打包说明

使用Visual Studio编译器或者dotnet命令进行打包，然后把打包文件放到服务器中最后注册服务即可，相关服务命令如下(**需管理员身份运行windows命令提示符**):  
```c#
✨删除服务: sc delete 服务名称
🛠举例: sc delete SychronizeTask     >>即可删除已注册的定时任务
```
```c#
✨停止服务: sc stop 服务名称
🛠举例: sc stop SychronizeTask     >>即可停止已注册的定时任务,也可在服务选项卡中停止
```
```c#
✨启动服务: sc start 服务名称
🛠举例: sc start SychronizeTask     >>即可开启已注册的定时任务，也可在服务选项卡中开启
```
```c#
✨注册服务: sc create [service name] [binPath= ] <option1> <option2>...
🍀命令说明:option1、option2为该命令可选的参数，比如下方的start、dispalayD等参数，另外，注册服务一定是指向.exe文件
🛠举例：sc create SychronizeTask binPath= "D:\Task\SychronizeTask.exe" start= auto displayname= "服务描述"              >>auto即为该服务是自启动服务
```
