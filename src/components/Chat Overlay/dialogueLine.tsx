import { DialogueEntry } from '@/lib/storylines/storyline-type'
import React from 'react'

const DialogueLine: React.FC<{ entry: DialogueEntry, text: string}> = ({ entry, text }) => {
  return (
    <div style={{
      ...styles.dialogueRow,
      ...(entry.source === 'narrator' ? {justifyContent: "center"} : (
        entry.source === 'enemy' ? {justifyContent: 'flex-end'} : {justifyContent: 'flex-start'}))
    }}>
      <div style={{
        ...styles.bubble,
        ...(entry.source === 'narrator' ? 
          {backgroundColor: 'rgba(70, 70, 70, 0.95)', fontStyle: 'italic'} : 
            (entry.source === 'enemy' ? {backgroundColor: 'rgba(130, 28, 28, 0.95)', minHeight: 90} : 
            {backgroundColor: 'rgba(30, 55, 120, 0.95)', minHeight: 90}
        ))
      }}>
        {entry.source !== 'narrator' && <div style={styles.img}/>}
        {text}
      </div>
    </div>
  )
}

export default DialogueLine

const styles: {[key: string]: React.CSSProperties} = {
  container: {
    display: 'flex'
  },
  dialogueRow: {
    display: 'flex',
    width: '100%',
  },
  bubble: {
    display: 'flex',
    flexDirection: 'row',
    maxWidth: '40%',
    borderRadius: 12,
    padding: '10px 12px',
    color: '#ffffff',
    lineHeight: 1.45,
    whiteSpace: 'pre-wrap',
    wordBreak: 'break-word',
  },
  img: {
    minHeight: 80,
    minWidth: 80,
    backgroundColor: '#a0a0a0',
    marginRight: 7,
  }
}