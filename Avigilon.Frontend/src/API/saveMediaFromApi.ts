import { MediaRequestContract } from "../../types/MediaRequestContract";

export async function SaveMediaFromApi(request: MediaRequestContract) {
  try {
    console.log("REQUEST", request);
    const response = await fetch("https://localhost:44390/saveMedia", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(request),
    });

    if (!response.ok) {
      const errorFromFetch = await response.json();
      return { errorMsg: errorFromFetch };
    } else {
      const data = await response.json();
      console.log(data);
      return { successMsg: data };
    }
  } catch (error) {
    if (error instanceof Error) {
        console.log(error)
      return { errorMsg: error.message };
    } else {
      return { errorMsg: "An error occurred while fetching data." };
    }
  }
}
