"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.StatisticsResolver = void 0;
//@Injectable()
//{
//  providedIn: 'root'
//}
var StatisticsResolver = /** @class */ (function () {
    function StatisticsResolver(http) {
        this.http = http;
    }
    StatisticsResolver.prototype.resolve = function (route, state) {
        debugger;
        var path = 'https://localhost:5001/api/Ticket/basicstats';
        var response = this.http.get(path, {
            headers: {
                'Authorization': "Bearer " + localStorage.getItem('authToken'),
            }
        });
        return response;
    };
    return StatisticsResolver;
}());
exports.StatisticsResolver = StatisticsResolver;
//# sourceMappingURL=statistics.resolver.js.map