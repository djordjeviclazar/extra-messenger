"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.TutorialdetailsResolver = void 0;
//@Injectable()
var TutorialdetailsResolver = /** @class */ (function () {
    function TutorialdetailsResolver(http) {
        this.http = http;
    }
    TutorialdetailsResolver.prototype.resolve = function (route, state) {
        var path = 'https://localhost:5001/api/tutorial/gettutorial/' + route.params['id'];
        var response = this.http.get(path, {
            headers: {
                'Authorization': "Bearer " + localStorage.getItem('authToken'),
            }
        });
        return response;
    };
    return TutorialdetailsResolver;
}());
exports.TutorialdetailsResolver = TutorialdetailsResolver;
//# sourceMappingURL=tutorialdetails.resolver.js.map