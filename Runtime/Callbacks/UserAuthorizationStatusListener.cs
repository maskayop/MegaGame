using RuStore.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuStore.PayClient.Internal {

    public class UserAuthorizationStatusListener : ResponseListener<UserAuthorizationStatus> {

        private const string javaClassName = "ru.rustore.unitysdk.payclient.callbacks.UserAuthorizationStatusListener";

        public UserAuthorizationStatusListener(Action<RuStoreError> onFailure, Action<UserAuthorizationStatus> onSuccess) : base(javaClassName, onFailure, onSuccess) {
        }

        protected override UserAuthorizationStatus ConvertResponse(AndroidJavaObject responseObject) {
            var response = UserAuthorizationStatus.AUTHORIZED;

            return response;
        }
    }
}
