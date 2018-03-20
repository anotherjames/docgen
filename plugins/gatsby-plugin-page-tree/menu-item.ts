export default class MenuItem {
    constructor() {
        this.path = "";
        this.title = "";
        this.selected = false;
        this.active = false;
        this.isEmptyParent = false;
        this.children = [];
    }
    path: string
    title: string
    selected: boolean
    active: boolean
    isEmptyParent: boolean
    children: Array<MenuItem>
}