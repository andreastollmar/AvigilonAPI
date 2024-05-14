import { useEffect, useRef, useState } from "react";
import type { Timelines } from "../../types/Timeline";
import type { TimelineRequestBody } from "../../types/TimelineRequestBody";

interface Props{
    requestBody: TimelineRequestBody | null,
}

export default function FetchTimeline({requestBody} : Props) {
  const [timelinesList, setTimelinesList] = useState<Timelines[] | null>(
    null,
  );
  const [errorMessage, setErrorMessage] = useState();
  const isMounted = useRef(false);

  useEffect(() => {
    isMounted.current = true;
    const requestOptions = {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(requestBody),
    };
    const fetchData = async () => {
      const response = await fetch("https://localhost:44390/getList", requestOptions);

      if (!response.ok) {
        const errorFromFetch = await response.json();
        setErrorMessage(errorFromFetch);
      } else {
        const data = (await response.json()) as Timelines[];
        if (isMounted.current) setTimelinesList(data);
      }
    };
    fetchData();
    return () => {
      isMounted.current = false;
    };
  }, []);
  return { timelinesList, errorMessage };
}
