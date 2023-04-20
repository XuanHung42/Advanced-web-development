import axios from "axios";
import React from "react";
import { useLocation } from "react-router-dom";

export function isEmplyOrSpaces(str){
    return str==null||(typeof str === 'string' && str.match(/^ *$/) !== null);
}

export function useQuery(){
    const{search}= useLocation();
    return React.useMemo(() =>  new   URLSearchParams(search), [search]); 
   }

   export function isInteger(str) {
    return Number.isInteger(Number(str)) && Number(str) >= 0;
    }

    export async function post_api(your_api, formData) {
        try {
        const response = await axios.post(your_api, formData);
        const data = response.data;
        if (data.isSuccess)
        return data.result;
        else
        return null;
        } catch (error) {
        console.log('Error', error.message);
        return null;
        }
        }

        export function decode(str) {
            let txt = new DOMParser().parseFromString(str, "text/html");
            return txt.documentElement.textContent;
        }