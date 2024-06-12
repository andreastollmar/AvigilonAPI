import "./App.css";
import { useState } from "react";
import { MediaRequestContract } from "../types/MediaRequestContract";
import { SaveMediaFromApi } from "./API/saveMediaFromApi";
import HeaderDisplay from "./components/Header";
import { SuccessMsg } from "../types/SuccessMsg";

function App() {
  const [mediaRequest, setMediaRequest] = useState<MediaRequestContract>({
    camera: '',
    isImg: false,
    requestBody: [{ date: '', time: '' }],
  });

  const [successMsg, setSuccessMsg] = useState<SuccessMsg | null>(null);
  const [errorMsg, setErrorMsg] = useState<string | undefined>();

  const handleAddRow = () => {
    setMediaRequest(prev => ({
      ...prev,
      requestBody: [...prev.requestBody, { date: '', time: '' }]
    }));
  };

  const handleRemoveRow = (index: number) => {
    setMediaRequest(prev => ({
      ...prev,
      requestBody: prev.requestBody.filter((_, idx) => idx !== index)
    }));
  };

  const handleChange = (index: number, event: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = event.target;
    setMediaRequest(prev => ({
      ...prev,
      requestBody: prev.requestBody.map((item, idx) => 
        idx === index ? { ...item, [name]: value } : item
      )
    }));
  };

  const handleCheckboxChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setMediaRequest(prev => ({
      ...prev,
      isImg: event.target.checked
    }));
  };

  const handleCameraChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setMediaRequest(prev => ({
      ...prev,
      camera: event.target.value
    }));
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setSuccessMsg(null);
    setErrorMsg(undefined);
    const result = await SaveMediaFromApi(mediaRequest);
    if (result.errorMsg) {
      setErrorMsg(result.errorMsg);
      setSuccessMsg(null);
    } else if (result.successMsg) {
      setSuccessMsg(result.successMsg);
      setErrorMsg(undefined);
    }
  };

  return (
    <>
      <HeaderDisplay />
      <div className="content">
        <form onSubmit={handleSubmit} className="form">
          {mediaRequest.requestBody.map((input, index) => (
            <div key={index} className="input_div">
              <input
                type="text"
                name="date"
                className="input_date"
                value={input.date}
                onChange={(event) => handleChange(index, event)}
                placeholder="Date"
              />
              <input
                type="text"
                name="time"
                className="input_time"
                value={input.time}
                onChange={(event) => handleChange(index, event)}
                placeholder="Time"
              />
              <button
                type="button"
                className="remove_button"
                onClick={() => handleRemoveRow(index)}
              >
                Remove
              </button>
            </div>
          ))}
          <button className="add_row_btn" type="button" onClick={handleAddRow}>
            Add Row
          </button>
          <div className="camera_img_div">
            <div>
              <input
                type="checkbox"
                name="isImg"
                className="input_isImg"
                checked={mediaRequest.isImg}
                onChange={handleCheckboxChange}
              />
              <label htmlFor="isImg">Img?</label>
            </div>
            <div>
              <label htmlFor="camera">Camera:</label>

              <input
                type="text"
                name="camera"
                className="input_camera"
                value={mediaRequest.camera}
                onChange={handleCameraChange}
                placeholder="Camera"
              />
            </div>
          </div>
          <div className="submit_btn_div">
            <button type="submit" className="submit_button">
              Save media files
            </button>
          </div>
          <div className="msg_div">
            {successMsg && 
            <div>{successMsg.fileSaved.map((file, index) => (
              <div>{file}</div>
            ))}
            <div>Files saved at {successMsg.saveLocation}</div>
            </div>}
            {/* {errorMsg && <div>{errorMsg}</div>} */}
          </div>
        </form>
      </div>
    </>
  );
}

export default App;
