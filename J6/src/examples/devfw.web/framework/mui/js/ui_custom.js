
//用户后台自定义文件
//window.menuHandler = function (data) {
//    data[0].childs.push({
//        text: '车辆管理2', uri: '', toggle: true, iconPos: '-168|0',
//        childs: [
//            { text: '车辆录入', uri: '/admin/car/AddCarProfile' }
//        ]
//    });
//}

window.menuHandler = function (data) {
    data[0].childs.push(
            {
                text: '<b style="color:green">微信管理</b>',
                uri: '',
                toggle: true,
                iconPos: '-168|0',
                childs: [
                    { text: '基本设置', uri: '/wxm/basicSetting' },
                    { text: '菜单编辑', uri: '/wxm/menu' },
                    { text: '素材管理', uri: '/wxm/resourceList' }
                ]
            });
};