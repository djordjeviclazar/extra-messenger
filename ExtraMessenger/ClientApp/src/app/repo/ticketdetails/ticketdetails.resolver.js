"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.TicketdetailsResolver = void 0;
//@Injectable()
var TicketdetailsResolver = /** @class */ (function () {
    function TicketdetailsResolver(http) {
        this.http = http;
    }
    TicketdetailsResolver.prototype.resolve = function (route, state) {
        var path = 'https://localhost:5001/api/ticket/getticket/' + route.params['id'];
        var response = this.http.get(path, {
            headers: {
                'Authorization': "Bearer " + localStorage.getItem('authToken'),
            }
        });
        return response;
    };
    return TicketdetailsResolver;
}());
exports.TicketdetailsResolver = TicketdetailsResolver;
//# sourceMappingURL=ticketdetails.resolver.js.map