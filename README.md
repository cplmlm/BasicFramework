基于 .Ne8 框架搭建的一个基础项目结构
采用最新版 .NET 8框架为基础搭建的一个项目框架，为了在开发一个新项目时能够快速的进入业务逻辑的开发，而不需要每次去重新构建一些项目基础的内容。
# 项目介绍
项目主要整合了如下一些常用的技术点
- API 的授权认证采用 JWT 认证方式
- 全局异常记录实现，中间件和过滤器都有实现
- Redis分布式缓存实现
- 接入国产数据库ORM组件SqlSugar，封装数据库操作
- 使用Autofac和系统DI批量注入接口和实现，可根据自己实际情况选择
- 支持国密SM2、MD5
- 使用 Automapper 处理对象映射
- 使用 Swagger 做api文档
# 项目目录
- Common——公共文件
- Extensions——第三方组件
- IServices——业务逻辑接口
- Services——业务逻辑实现
- Repository——仓储类
- Model——实体类
- WebAPI——接口

