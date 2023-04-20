import { get_api } from "./Method";
export async function getCategories(){
  return get_api(`https://localhost:7278/api/categories`);
        
    }
