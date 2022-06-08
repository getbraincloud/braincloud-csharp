var NativeWebSocket = {

    $WebSocketInstances: [],

    BrainCloudSocketCreate: function (url, id) {
    	console.log("[NativeWebSocket] Socket create: " + id);
        var str = Pointer_stringify(url);
        var webSocket = {
        	socket: new WebSocket(str),
        	id: id,
        	error: null,
        	messages: []
        };
        
        webSocket.socket.binaryType = "arraybuffer";

        WebSocketInstances[id] = webSocket;

        console.log(WebSocketInstances);

        console.log(WebSocketInstances[id]);
    },

    BrainCloudSocketClose: function (id) {
    	console.log("[NativeWebSocket] Socket close: " + id);
        WebSocketInstances[id].socket.close();
        delete WebSocketInstances[id];
    },

    BrainCloudSocketSend: function (ptr, length, id) {
    	console.log("[NativeWebSocket] Socket send: " + id);
        WebSocketInstances[id].socket.send(HEAPU8.buffer.slice(ptr, ptr + length));
    },

    BrainCloudSocketState: function (id) {
    	console.log("[NativeWebSocket] Socket state: " + id);
        return WebSocketInstances[id].socket.readyState;
    },

    BrainCloudSocketOnOpen: function(action, id){
    	console.log("[NativeWebSocket] Set socket on open: " + id);
    	console.log(WebSocketInstances[id]);
    	WebSocketInstances[id].socket.onopen = function(){
    		console.log("[NativeWebSocket] Socket on open: " + id);
    		Module['dynCall_vi'](action, id);
    	}
    },

    BrainCloudSocketOnMessage: function(action, id){
    	console.log("[NativeWebSocket] Set socket on message: " + id);
    	WebSocketInstances[id].socket.onmessage = function(e){
    		console.log("[NativeWebSocket] Socket on message: " + id);
			if (e.data instanceof Blob) {
				var reader = new FileReader();
				reader.addEventListener("loadend", function() {
					var array = new Uint8Array(reader.result);
					WebSocketInstances[id].messages.push(array);
					Module['dynCall_vi'](action, id);
				});
				reader.readAsArrayBuffer(e.data);
			} else if (e.data instanceof ArrayBuffer) {
				var array = new Uint8Array(e.data);
				WebSocketInstances[id].messages.push(array);
				Module['dynCall_vi'](action, id);
			}
    	}
    },

	BrainCloudSocketOnError: function(action, id){
		console.log("[NativeWebSocket] Set socket on error: " + id);
    	WebSocketInstances[id].socket.onerror = function(e){
    		console.log("[NativeWebSocket] Socket on error: " + id);
			WebSocketInstances[id].error = e.message;
			Module['dynCall_vi'](action, id);
    	}
    },

    BrainCloudSocketOnClose: function(action, id){
    	console.log("[NativeWebSocket] Set socket on close: " + id);
    	WebSocketInstances[id].socket.onclose = function(e){
    		console.log("[NativeWebSocket] Socket on close: " + id);
			Module['dynCall_vi'](action, e.code, id); 
    	}
    },

    BrainCloudSocketReceiveLength: function(id){
    	console.log("[NativeWebSocket] Socket receive length: " + id);
		if (WebSocketInstances[id].messages.length == 0) return 0;
		return WebSocketInstances[id].messages[0].length;
	},

	BrainCloudSocketReceive: function(ptr, length, id){
		console.log("[NativeWebSocket] Socket receive: " + id);
		if (WebSocketInstances[id].messages.length == 0) return 0;
		if (WebSocketInstances[id].messages[0].length > length) return 0;
		HEAPU8.set(WebSocketInstances[id].messages[0], ptr);
		WebSocketInstances[id].messages = WebSocketInstances[id].messages.slice(1);
	},

	BrainCloudSocketError: function(ptr, buffersize, id){
		console.log("[NativeWebSocket] Socket error: " + id);
	 	if (WebSocketInstances[id].error == null) return 0;
	    var str = WebSocketInstances[id].error.slice(0, Math.max(0, buffersize - 1));
	    writeStringToMemory(str, ptr, false);
		return 1;
	}
};

autoAddDeps(NativeWebSocket, '$WebSocketInstances');
mergeInto(LibraryManager.library, NativeWebSocket);