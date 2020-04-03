// The following sample code uses modern ECMAScript 6 features 
// that aren't supported in Internet Explorer 11.
// To convert the sample for environments that do not support ECMAScript 6, 
// such as Internet Explorer 11, use a transpiler such as 
// Babel at http://babeljs.io/.
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};


var connection = new signalR.HubConnectionBuilder()
    .withUrl("/streamHub")
    .build();

//var cmd = "dir c:\\*.png";
//var cmd = "redis-benchmark -q  -h 127.0.0.1 -p 6379 -t set,get -c 50 -n 100000";
var cmd;
//var d = [];


$("#wndStartTestModal .btn-primary").on("click", (event) => __awaiter(this, void 0, void 0, function* () {
    
    $("#wndStartTestModal").modal('hide');

    var p = document.createElement("p");
    p.setAttribute("class", "console");
    p.style.color = "#2196F3";
    p.textContent = cmd;
    document.getElementsByClassName("terminal-home")[0].appendChild(p);
    p.scrollIntoView();

    var counter = 0;
    let reg = /(SET|GET):\s(\d+\.\d+)/;

    try {

        connection.stream("RunCommand", cmd)
            .subscribe({
                next: (item) => {
                    var p = document.createElement("p");
                    p.setAttribute("class", "console");
                    p.textContent = item;
                    document.getElementsByClassName("terminal-home")[0].appendChild(p);
                    p.scrollIntoView();


                    var matchResult = reg.exec(item);
                    if (matchResult && matchResult.length === 3) {
                        $("#wndStartTestModal").trigger("data:received",[matchResult[1], counter++, matchResult[2]]);
                       // d.push([matchResult[1], counter, matchResult[2]]);
                    }


                },
                complete: () => {
                    var p = document.createElement("p");
                    p.setAttribute("class", "console text-danger");
                    p.textContent = "==============================================";
                    document.getElementsByClassName("terminal-home")[0].appendChild(p);
                    p.scrollIntoView();
                    $("#wndStartTestModal").trigger("test:completed");
                },
                error: (err) => {
                    var li = document.createElement("li");
                    li.textContent = err;
                    document.getElementById("messagesList").appendChild(li);
                }
            });

    }
    catch (e) {
        console.error(e.toString());
    }
    event.preventDefault();
}));
// We need an async function in order to use await, but we want this code to run immediately,
// so we use an "immediately-executed async function"
(() => __awaiter(this, void 0, void 0, function* () {
    try {
        yield connection.start();
    }
    catch (e) {
        console.error(e.toString());
    }
}))();