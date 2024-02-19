using UnityEngine;
using System.Collections;
using DPLogin;
using Google.Protobuf;

public class ProtoBufDemo : MonoBehaviour {
    // Use this for initialization
    void Start() {
        LoginRequest();
        LoginResponse();
    }

    private object GetProtoObject(ProtoEventName eventName) {
        if (ProtoEventNameTool.ProtoMap.TryGetValue((int)eventName, out var type)) {
            var obj = System.Activator.CreateInstance(type);
            return obj;
        }
        return null;
    }

    private object ParseProtoObject(byte[] bytes, ProtoEventName eventName) {
        if (ProtoEventNameTool.ProtoMap.TryGetValue((int)eventName, out var type)) {
            var obj = System.Activator.CreateInstance(type);
            var parser = obj as IMessage;
            parser.MergeFrom(bytes);
            return obj;
        }
        return null;
    }

    private void LoginRequest() {
        var obj = GetProtoObject(ProtoEventName.DPUserLoginInfoReq);
        DPUserLoginInfoReq userLoginInfo = obj as DPUserLoginInfoReq;
        userLoginInfo.SzAccount = "test user";
        userLoginInfo.SzPassword = "123456789";
        userLoginInfo.SzMacAdress = "123456789";

        var bytes = userLoginInfo.ToByteArray();
        var loginInfo = ParseProtoObject(bytes, ProtoEventName.DPUserLoginInfoReq) as DPUserLoginInfoReq;
        Debug.Log(loginInfo.SzAccount);
        Debug.Log(loginInfo.SzPassword);
    }

    private void LoginResponse() {
        DPAccountVerifyResultResp accountVerifyResultResp = new DPAccountVerifyResultResp();
        accountVerifyResultResp.SzAccount = "test user";
        accountVerifyResultResp.NResult = AccountLoginVerifyResult.AccVerifyRetClientversionerror;

        var bytes = accountVerifyResultResp.ToByteArray();
        var accountVerifyResultRespReceived = DPAccountVerifyResultResp.Parser.ParseFrom(bytes);

        Debug.Log(accountVerifyResultRespReceived.SzAccount);
        Debug.Log(accountVerifyResultRespReceived.NResult);
    }
}