export function isEmplyOrSpaces(str){
    return str==null||(typeof str === 'string' && str.match(/^ *$/) !== null);
}