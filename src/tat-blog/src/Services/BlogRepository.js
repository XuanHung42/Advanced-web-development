import axios from "axios";
import { get_api } from "./Method";
import { post_api } from "../Utils/Utils";
export async function getPosts(params, pageSize=3, pageNumber=1){

        
        return    get_api(`https://localhost:7278/api/posts?PageSize=${pageSize}&PageNumber=${pageNumber}`);
            // axios.get(`https://localhost:7278/api/posts?`,{params})
            
    }
    export async function getPostById (id=0){
      return get_api(`https://localhost:7278/api/posts/${id}`);
       
     }

    export async function getPostBySlug (urlSlug = ''){
       return get_api(`https://localhost:7278/api/posts/slug/${urlSlug}`);
        
      }
      export async function getPostByCateSlug (urlSlug = '', pageSize=3, pageNumber=1){
       return get_api(`https://localhost:7278/api/categories/${urlSlug}/posts?PageSize=${pageSize}&PageNumber=${pageNumber}`);
         
        
      }
      export async function getPostByAuthorSlug (urlSlug = '', pageSize=3, pageNumber=1){
        return get_api(`https://localhost:7278/api/authors/${urlSlug}/posts?PageSize=${pageSize}&PageNumber=${pageNumber}`);
         
      }
      export async function getPostByTagSlug (urlSlug = '', pageSize=3, pageNumber=1){
       return get_api(`https://localhost:7278/api/tags/${urlSlug}?PageSize=${pageSize}&PageNumber=${pageNumber}`);
         
      }
      export async function getPostsFeature(limit=3){
        return get_api(`https://localhost:7278/api/posts/featured/${limit}`);
            
        
    }
    export async function getPostsRadom(limit=3){
      return get_api(`https://localhost:7278/api/posts/random/${limit}`);
          
      
  }

  export function getAuthors(name='', pageSize=3, pageNumber=1){
    return   get_api(`https://localhost:7278/api/authors?Name=${name}&PageSize=${pageSize}&PageNumber=${pageNumber}`)
  }

  export function getFilter(){
    return get_api(`https://localhost:7278/api/posts/get-filter`)
  }
  export function getPostsFilter(keyword = '', authorId = '', categoryId = '',
year = '', month = '', pageSize = 10, pageNumber = 1, sortColumn = '',
sortOrder = '') {
let url = new URL('https://localhost:7278/api/posts/get-posts-filter');
keyword !== '' && url.searchParams.append('Keyword', keyword);
authorId !== '' && url.searchParams.append('AuthorId', authorId);
categoryId !== '' && url.searchParams.append('CategoryId', categoryId);
year !== '' && url.searchParams.append('Year', year);
month !== '' && url.searchParams.append('Month', month);
sortColumn !== '' && url.searchParams.append('SortColumn', sortColumn);
sortOrder !== '' && url.searchParams.append('SortOrder', sortOrder);
url.searchParams.append('PageSize', pageSize);
url.searchParams.append('PageNumber', pageNumber);
return get_api(url.href);
}

export function addOrUpdatePost(formData){
  return post_api('https://localhost:7278/api/posts', formData);
  }