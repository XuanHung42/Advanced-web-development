import axios from "axios";
export async function getPosts(params, pageSize=3, pageNumber=1){

        try{
            const response = await 
            axios.get(`https://localhost:7278/api/posts?PageSize=${pageSize}&PageNumber=${pageNumber}`);
            // axios.get(`https://localhost:7278/api/posts?`,{params})
            const data = response.data;
            if(data.isSuccess)
                return data.result;
            else
                 return null;

        }catch(error){
                console.log('Error', error.message);
                return null;
        }
    }
    export async function getPostBySlug (urlSlug = ''){
        try {
          const response = await axios.get(`https://localhost:7278/api/posts/slug/${urlSlug}`);
          const data = response.data

          if (data.isSuccess){
            return data.result;
          }
          else{
            return null;
          }
        } catch (error) {
          console.log('Error', error.message);
          return null
        }
      }
      export async function getPostByCateSlug (urlSlug = '', pageSize=3, pageNumber=1){
        try {
          const response = await axios.get(`https://localhost:7278/api/categories/${urlSlug}/posts?PageSize=${pageSize}&PageNumber=${pageNumber}`);
          const data = response.data
         
          if (data.isSuccess){
            return data.result;
          }
          else{
            return null;
          }
        } catch (error) {
          console.log('Error', error.message);
          return null
        }
      }
      export async function getPostByAuthorSlug (urlSlug = '', pageSize=3, pageNumber=1){
        try {
          const response = await axios.get(`https://localhost:7278/api/authors/${urlSlug}/posts?PageSize=${pageSize}&PageNumber=${pageNumber}`);
          const data = response.data
   
          if (data.isSuccess){
            return data.result;
          }
          else{
            return null;
          }
        } catch (error) {
          console.log('Error', error.message);
          return null
        }
      }
      export async function getPostByTagSlug (urlSlug = '', pageSize=3, pageNumber=1){
        try {
          const response = await axios.get(`https://localhost:7278/api/tags/${urlSlug}?PageSize=${pageSize}&PageNumber=${pageNumber}`);
          const data = response.data
          
          if (data.isSuccess){
            return data.result;
          }
          else{
            return null;
          }
        } catch (error) {
          console.log('Error', error.message);
          return null
        }
      }
      export async function getPostsFeature(limit=3){
        try{
            const response= await axios.get(`https://localhost:7278/api/posts/featured/${limit}`);
            const data = response.data;
            if(data.isSuccess){
                return data.result;
            }
            else
                return null;
        }catch(error){
                console.log('Error', error.message);
                return null;
        }
    }
    export async function getPostsRadom(limit=3){
      try{
          const response= await axios.get(`https://localhost:7278/api/posts/random/${limit}`);
          const data = response.data;
          if(data.isSuccess){
              return data.result;
          }
          else
              return null;
      }catch(error){
              console.log('Error', error.message);
              return null;
      }
  }