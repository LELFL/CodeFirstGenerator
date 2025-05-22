window.api = {
    baseUrl: window.location.origin,
  // 全局 api 请求适配器
  // 另外在 amis 配置项中的 api 也可以配置适配器，针对某个特定接口单独处理。

  requestAdaptor(api) {
    // 支持异步，可以通过 api.mockResponse 来设置返回结果，跳过真正的请求发送
    // 此功能自定义 fetcher 的话会失效
    // api.context 中包含发送请求前的上下文信息
    if (api.url.startsWith("/api")) {
      api.url = window.api + api.url;
    }

    return api;
  },

  // 全局 api 适配器。
  // 另外在 amis 配置项中的 api 也可以配置适配器，针对某个特定接口单独处理。
  responseAdaptor(api, payload, query, request, response) {
    if (
      payload.status != undefined &&
      payload.msg != undefined &&
      payload.data != undefined
    ) {
      return payload;
    }
    if (payload.error) {
      var msg = payload.error.details || payload.error.message || "未知错误！";
      console.log(payload.error);
      return {
        status: -1,
        msg: msg,
        data: payload,
      };
    }

    if (response.status >= 200 && response.status < 300) {
      return {
        status: 0,
        msg: "",
        data: payload,
      };
    } else {
      return {
        status: -1,
        msg: response.status,
        data: payload,
      };
    }

    console.log(api, payload, query, request, response);
    return payload;
  },
};
