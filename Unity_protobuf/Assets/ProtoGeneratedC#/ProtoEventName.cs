//-----------Auto Generate ByTools-----------
//-----------Time:2024-02-18T15:17:31-----------
//-----------Don't change-----------

using System;
using System.Collections.Generic;

public enum ProtoEventName {
    /// <summary>
    /// 请求账号信息
    /// </summary>
    DPUserLoginInfoReq = 200001,

    /// <summary>
    /// 客户端发起登陆协议
    /// </summary>
    DPGSLogicLoginReq = 200002,

    /// <summary>
    /// 客户端登陆逻辑服失败返回错误码
    /// </summary>
    DPGSLogicLoginRetResp = 100002,

    /// <summary>
    /// 玩家确定选取角色登录游戏
    /// </summary>
    DPGameLoginRequestReq = 100003,

    /// <summary>
    /// 创建角色发送创建的角色名到网关服务器
    /// </summary>
    DPCreateRoleReq = 200004,

    /// <summary>
    /// 创建角色返回结果给客户端
    /// </summary>
    DPCreateRoleResultResp = 100004,

    /// <summary>
    /// 踢掉客户端
    /// </summary>
    DPKickOutClientResp = 200005,

    /// <summary>
    /// 测试
    /// </summary>
    DPTest = 200008,

    /// <summary>
    /// 案例一
    /// </summary>
    Card = 200001,

    /// <summary>
    /// 案例二
    /// </summary>
    Player = 200002,

    /// <summary>
    /// 案例三
    /// </summary>
    Item = 200003,
}
//-----------End-----------

public static class ProtoEventNameTool {
    public static Dictionary<int, Type> ProtoMap = new() {
        { 200001, typeof(DPLogin.DPUserLoginInfoReq) },
        { 200002, typeof(DPLogin.DPGSLogicLoginReq) },
        { 100002, typeof(DPLogin.DPGSLogicLoginRetResp) },
        { 100003, typeof(DPLogin.DPGameLoginRequestReq) },
        { 200004, typeof(DPLogin.DPCreateRoleReq) },
        { 100004, typeof(DPLogin.DPCreateRoleResultResp) },
        { 200005, typeof(DPLogin.DPKickOutClientResp) },
        { 200008, typeof(DPLogin.DPTest) },
    };
}