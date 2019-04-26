const apiURL = 'http://localhost:5000/api/'
//onst apiURL = 'https://thedbateproject.azurewebsites.net/backend/api/'
const KFCURL = 'https://kfc-sso.com'

const URL = {
    randQuestURL: apiURL + 'question/randomquestion',
    sendMsgURL: apiURL + 'chat/postmessage',
    displayTelLogsURL: apiURL + 'log/displaytelemetrylogs',
    displayErrorLogsURL: apiURL + 'log/displayerrorlogs',
    deleteErrorLogsURL: apiURL + 'log/deletelog',
    publishAppURL: KFCURL + '/api/applications/publish',
    getQuestsURL: apiURL + 'question/getquestions',
    deleteQuestURL: apiURL + 'question/delete',
    addQuestURL: apiURL + 'question/add',
    getUserURL: apiURL + 'user/getuser',
    postIPURL: apiURL + ''
}

export {
    URL,
    KFCURL
}