#!/bin/bash
WORKSPACE_PATH=`cygpath -u "${WORKSPACE}"`
REVIEW_MESSAGE_FILE="${WORKSPACE_PATH}/REVIEW_MESSAGE"

executelogandtime() {
  COMMAND=$1
  TEXT=$2
  START=$(date +%s)
  eval $COMMAND
  RESULT=$?
  END=$(date +%s)
  TIME=$(( $END - $START ))
  MINUTES=$(( $TIME / 60 ))
  SECONDS=$(( $TIME % 60 ))

  if [ $RESULT -eq 0 ] ; then
    echo "$TEXT succeeded in ${MINUTES}m${SECONDS}s." >> $REVIEW_MESSAGE_FILE
    return $RESULT
  else
    echo "$TEXT failed in ${MINUTES}m${SECONDS}s." >> $REVIEW_MESSAGE_FILE
    exit $RESULT
  fi
}

BASE_DIR=`dirname $0`
if [ "$1" = "end" ]; then
    cd $BASE_DIR/../OpenPass.IdController.UI || return
    UI_CHANGE_COUNT=`git diff-tree --no-commit-id --name-only -r HEAD | grep OpenPass.IdController.UI | wc -l`

    if [ $UI_CHANGE_COUNT -ne 0 ]; then
        executelogandtime 'npm run lint' 'Angular app lint'
        executelogandtime 'npm run test' 'Angular app test'
        executelogandtime 'npm run cypress:run-tests' 'Cypress test'
    else
        echo "No change in UI source code, integration tests will not be triggered."
    fi
fi
