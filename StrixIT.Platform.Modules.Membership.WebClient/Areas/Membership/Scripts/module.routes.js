﻿//#region Apache License
/**
 * Copyright 2015 StrixIT. Author R.G. Schurgers MA MSc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
//#endregion

strixIT.membership = {};

strixIT.membership.loadUserCallBack = function (data) {
    var today = new Date();

    for (var n in data.roles) {
        var role = data.roles[n];

        if (role.startDate < role.groupStartDate) {
            role.startDate = role.groupStartDate
        }

        if (role.startDate < today) {
            role.startDate = today;
        }

        if (role.endDate && role.groupEndDate && role.endDate > role.groupEndDate) {
            role.endDate = role.groupEndDate
        }

        if (role.endDate && role.startDate > role.endDate) {
            role.startDate = role.endDate;
        }
    }
}

adminModule.addCrudRoute({ module: 'Membership', type: 'User', loadCallBack: strixIT.membership.loadUserCallBack, dependencies: ["Areas/Membership/Scripts/strixit.membership.controller.js"] });
adminModule.addCrudRoute({ module: 'Membership', type: 'Group', dependencies: ["Areas/Membership/Scripts/strixit.membership.controller.js"] });
adminModule.addCrudRoute({ module: 'Membership', type: 'Role', dependencies: ["Areas/Membership/Scripts/strixit.membership.controller.js"] });