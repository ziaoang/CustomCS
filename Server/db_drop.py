print 'Please input "customcs" to confirm you know what you are doing'
confirm = raw_input()

if confirm == 'customcs':
    from index import db
    db.drop_all()


