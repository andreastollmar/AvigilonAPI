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
      const dataFromFetch = await response.json();
      console.log(dataFromFetch);
      return { successMsg: dataFromFetch};
    }
  } catch (error) {
    console.log(error as string)
    if (error instanceof Error) {
      return { errorMsg: error.message };
    } else {
      return { errorMsg: "An error occurred while fetching data." };
    }
  }
}
